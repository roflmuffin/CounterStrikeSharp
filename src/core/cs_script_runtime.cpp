#include "core/cs_script_runtime.h"
#include "core/globals.h"
#include "scripting/script_engine.h"
#include <stdexcept>
#include <thread>

#ifdef CSS_ENABLE_CS_SCRIPT
#include "core/gameconfig.h"
#include "core/hooks.h"
#include "core/log.h"
#include "core/cs_script_descriptors.h"
#include <entity2/entityinstance.h>
#include <entity2/entitysystem.h>
#include <v8.h>
#include <cstring>
#include <limits>
#include <unordered_map>
#include <vector>
#include <memory>
#include <filesystem>
#include <link.h>
#include <dlfcn.h>

namespace counterstrikesharp::cs_script {
namespace {
using Local = v8::Local<v8::Value>;
static_assert(sizeof(v8::HandleScope) == 24 && sizeof(v8::TryCatch) == 48);
static_assert(v8::internal::Internals::kIsolateRootsOffset == 0x240);
static_assert(v8::internal::kApiTaggedSize == 8);

struct Adapter
{
    CGameConfig profile;
    using GetContext = v8::Local<v8::Context> (*)(void*);
    GetContext context;
    void (*push)(void*);
    void (*pop)();
    void* (*top)();
    Local (*wrap)(CEntityInstance*);
    void* (*unwrap)(v8::Object*);
    void* moneyCallback;
    void (*registerApi)(void*);
    CEntityInstance* (*createHost)(const char*, int);
    void (*spawnHost)(void*, void*);
    void (*removeHost)(CEntityInstance*);
    v8::Module::ResolveModuleCallback resolveModule;
    int scriptOffset, contextOffset, incarnationOffset, exportCountOffset;

    Adapter() : profile((std::filesystem::path(globals::gameConfig->GetPath()).parent_path() / "cs_script.linux.json").string())
    {
        char error[256]{};
        if (!profile.Init(error, sizeof(error))) throw std::runtime_error(error);
        if (std::strcmp(v8::V8::GetVersion(), "11.9.169.6") != 0)
            throw std::runtime_error("Unsupported game V8 version; expected 11.9.169.6");
        auto resolve = [this](const char* name) {
            auto result = profile.ResolveSignature(name);
            if (!result) throw std::runtime_error(std::string("Missing engine signature: ") + name);
            return result;
        };
        context = reinterpret_cast<GetContext>(resolve("CsScript_GetContext"));
        push = reinterpret_cast<decltype(push)>(resolve("CsScript_Push"));
        pop = reinterpret_cast<decltype(pop)>(resolve("CsScript_Pop"));
        top = reinterpret_cast<decltype(top)>(resolve("CsScript_Top"));
        wrap = reinterpret_cast<decltype(wrap)>(resolve("CsScript_WrapEntity"));
        unwrap = reinterpret_cast<decltype(unwrap)>(resolve("CsScript_Unwrap"));
        moneyCallback = resolve("CsScript_AddTeamMoneyCallback");
        registerApi = reinterpret_cast<decltype(registerApi)>(resolve("CsScript_RegisterApi"));
        resolveModule = reinterpret_cast<decltype(resolveModule)>(resolve("CsScript_ResolveModule"));
        // This signature includes the embedded-script displacement and call to
        // its constructor; an incompatible entity layout must fail closed.
        resolve("CsScript_HostLayout");
        scriptOffset = profile.GetOffset("CsScript_HostScript");
        contextOffset = profile.GetOffset("CsScript_Context");
        incarnationOffset = profile.GetOffset("CsScript_Incarnation");
        exportCountOffset = profile.GetOffset("CsScript_ExportCount");
        if (scriptOffset != 0x7a0 || contextOffset != 0x28 || incarnationOffset != 0x20 || exportCountOffset != 0x38)
            throw std::runtime_error("Unsupported cs_script layout profile");
        Dl_info info{};
        if (!dladdr(reinterpret_cast<void*>(context), &info)) throw std::runtime_error("Cannot identify server module");
        struct BuildId { void* base; std::string value; } buildId{info.dli_fbase, {}};
        dl_iterate_phdr([](dl_phdr_info* image, size_t, void* data) {
            auto& id = *static_cast<BuildId*>(data);
            if (reinterpret_cast<void*>(image->dlpi_addr) != id.base) return 0;
            for (size_t i = 0; i < image->dlpi_phnum; ++i)
            {
                const auto& ph = image->dlpi_phdr[i];
                if (ph.p_type != PT_NOTE) continue;
                auto* cursor = reinterpret_cast<const unsigned char*>(image->dlpi_addr + ph.p_vaddr);
                auto* end = cursor + ph.p_memsz;
                while (size_t(end - cursor) >= sizeof(ElfW(Nhdr)))
                {
                    ElfW(Nhdr) note;
                    std::memcpy(&note, cursor, sizeof(note));
                    cursor += sizeof(note);
                    size_t nameSize = (size_t(note.n_namesz) + 3) & ~size_t(3);
                    size_t descSize = (size_t(note.n_descsz) + 3) & ~size_t(3);
                    if (nameSize + descSize > size_t(end - cursor)) break;
                    if (note.n_type == NT_GNU_BUILD_ID && note.n_namesz == 4 && std::memcmp(cursor, "GNU", 4) == 0)
                    {
                        static constexpr char hex[] = "0123456789abcdef";
                        for (size_t j = 0; j < note.n_descsz; ++j)
                        {
                            auto byte = cursor[nameSize + j];
                            id.value += hex[byte >> 4]; id.value += hex[byte & 15];
                        }
                    }
                    cursor += nameSize + descSize;
                }
            }
            return 1;
        }, &buildId);
        auto* expected = profile.GetPatch("CsScript_ServerBuildId");
        if (!expected || buildId.value != expected) throw std::runtime_error("Unsupported libserver.so build ID: " + buildId.value);
        auto resolveEntity = [](const char* name) {
            auto* result = globals::gameConfig->ResolveSignature(name);
            if (!result) throw std::runtime_error(std::string("Missing entity signature: ") + name);
            return result;
        };
        createHost = reinterpret_cast<decltype(createHost)>(resolveEntity("UTIL_CreateEntityByName"));
        spawnHost = reinterpret_cast<decltype(spawnHost)>(resolveEntity("CBaseEntity_DispatchSpawn"));
        removeHost = reinterpret_cast<decltype(removeHost)>(resolveEntity("UTIL_Remove"));
    }
};
Adapter& Engine() { static Adapter engine; return engine; }

struct Session
{
    uint32_t host;
    void* script;
    void* contextSlot;
    uint32_t incarnation;
    uint64_t id;
    v8::Global<v8::Module> module;
    bool ready = false;
};
std::shared_ptr<Session> currentSession;
bool mapActive = false, initializationPending = false;
bool completedSimulationFrame = false;
uint64_t mapGeneration = 0;
std::string initializationError;
uint64_t nextId = 1; // Never reused across maps or host deletion.
HookSet observation;
uint64_t observedSession = 0, callbackEntries = 0;

CEntityInstance* Entity(uint32_t raw)
{
    if (!globals::entitySystem) throw std::runtime_error("Entity system is unavailable");
    CEntityHandle handle(raw);
    auto* entity = globals::entitySystem->GetEntityInstance(handle);
    if (!entity || entity->GetRefEHandle().ToInt() != raw)
        throw std::runtime_error("Deleted entity or stale entity serial");
    return entity;
}
void Validate(const Session& session)
{
    auto* host = Entity(session.host);
    auto& engine = Engine();
    auto* script = reinterpret_cast<char*>(host) + engine.scriptOffset;
    if (script != session.script || *reinterpret_cast<uint32_t*>(script + engine.incarnationOffset) != session.incarnation ||
        *reinterpret_cast<void**>(script + engine.contextOffset) != session.contextSlot)
        throw std::runtime_error("Stale cs_script session; host context was rebuilt");
}
struct Scope
{
    v8::Isolate* isolate;
    v8::Isolate::Scope isolateScope;
    v8::HandleScope handles;
    v8::Local<v8::Context> context;
    v8::Context::Scope contextScope;
    void* previous;

    static v8::Isolate* GetIsolate(const Session& session)
    {
        Validate(session);
        // Engine persistent handles are indirect slots in this verified ABI.
        // Context::GetIsolate does not allocate. GetContext then makes a fresh
        // local inside our handle scope; the borrowed slot is never retained.
        if (!session.contextSlot) throw std::runtime_error("Host has no V8 context");
        return reinterpret_cast<v8::Context*>(session.contextSlot)->GetIsolate();
    }
    explicit Scope(const Session& session)
        : isolate(GetIsolate(session)), isolateScope(isolate), handles(isolate),
          context(Engine().context(session.script)), contextScope(context), previous(Engine().top())
    {
        Engine().push(session.script);
    }
    ~Scope() { Engine().pop(); }
    Scope(const Scope&) = delete;
};

v8::Local<v8::String> String(v8::Isolate* isolate, const std::string& text)
{
    v8::Local<v8::String> result;
    if (text.size() > 1024 * 1024 || !v8::String::NewFromUtf8(isolate, text.data(), v8::NewStringType::kNormal,
                                                          static_cast<int>(text.size())).ToLocal(&result))
        throw std::runtime_error("Cannot allocate cs_script string");
    return result;
}
std::string Utf8(v8::Isolate* isolate, Local value)
{
    v8::String::Utf8Value text(isolate, value);
    return *text ? std::string(*text, text.length()) : "<unprintable script exception>";
}
Local Get(Scope& scope, v8::Local<v8::Object> object, const std::string& key)
{
    Local value;
    if (!object->Get(scope.context, String(scope.isolate, key)).ToLocal(&value))
        throw std::runtime_error("Script property access failed: " + key);
    return value;
}
std::string Exception(Scope& scope, v8::TryCatch& caught, const std::string& fallback)
{
    if (!caught.HasCaught()) return fallback;
    std::string result = Utf8(scope.isolate, caught.Exception());
    Local stack;
    if (caught.StackTrace(scope.context).ToLocal(&stack) && stack->IsString()) result += "\n" + Utf8(scope.isolate, stack);
    return result;
}
v8::Local<v8::Object> Exports(Scope& scope, const Session& session)
{
    if (session.module.IsEmpty()) throw std::runtime_error("Host module is not initialized");
    auto module = session.module.Get(scope.isolate);
    if (module->GetStatus() != v8::Module::kEvaluated) throw std::runtime_error("Host module is not evaluated");
    return module->GetModuleNamespace().As<v8::Object>();
}
v8::Local<v8::Object> Export(Scope& scope, const Session& session, v8::Local<v8::Object> bridge, const std::string& name)
{
    auto value = Get(scope, bridge, name);
    if (!value->IsObject()) throw std::runtime_error("Missing module export: " + name);
    auto object = value.As<v8::Object>();
    auto* binding = static_cast<char*>(Engine().unwrap(*object));
    if (!binding || *reinterpret_cast<int*>(binding) != 1 || *reinterpret_cast<void**>(binding + 8) != session.script)
        throw std::runtime_error("Module export belongs to a different script host: " + name);
    return object;
}
const MethodDescriptor& Description(uint32_t hash)
{
    static const auto methods = [] {
        std::unordered_map<uint32_t, const MethodDescriptor*> result;
        for (const auto& descriptor : Descriptors)
            if (!result.emplace(descriptor.id, &descriptor).second)
                throw std::runtime_error("Duplicate generated cs_script method hash");
        return result;
    }();
    auto it = methods.find(hash);
    if (it == methods.end()) throw std::runtime_error("Unknown cs_script method hash");
    return *it->second;
}

std::shared_ptr<Session> InitializeHost(uint32_t hostHandle)
{
    auto* host = Entity(hostHandle);
    auto& engine = Engine();
    auto* script = reinterpret_cast<char*>(host) + engine.scriptOffset;
    auto owned = std::make_shared<Session>();
    auto& session = *owned;
    session.host = hostHandle;
    session.script = script;
    session.contextSlot = *reinterpret_cast<void**>(script + engine.contextOffset);
    session.incarnation = *reinterpret_cast<uint32_t*>(script + engine.incarnationOffset);
    session.id = nextId++;
    Scope scope(session);
    v8::TryCatch caught(scope.isolate);
    try
    {
        if (*reinterpret_cast<int*>(script + engine.exportCountOffset) != 0)
            throw std::runtime_error("Expected a fresh, empty point_script host");
        // The engine's CCSPointScript registration virtual builds its
        // per-script templates and enum exports. No resource is loaded.
        CSSHARP_CORE_INFO("Shared cs_script initialization: registering engine API");
        engine.registerApi(script);
        CSSHARP_CORE_INFO("Shared cs_script initialization: resolving built-in module");
        v8::Local<v8::Module> module;
        if (!engine.resolveModule(scope.context, String(scope.isolate, "cs_script/point_script"), {}, {}).ToLocal(&module))
            throw std::runtime_error("The engine did not resolve its built-in module");
        if (!module->IsSyntheticModule() || !module->InstantiateModule(scope.context, engine.resolveModule).FromMaybe(false))
            throw std::runtime_error("Could not instantiate the engine's synthetic module");
        Local result;
        if (!module->Evaluate(scope.context).ToLocal(&result) || module->GetStatus() != v8::Module::kEvaluated)
            throw std::runtime_error("Could not evaluate the engine's synthetic module");
        if (result->IsPromise() && result.As<v8::Promise>()->State() != v8::Promise::kFulfilled)
            throw std::runtime_error("Engine module evaluation did not finish synchronously");
        session.module.Reset(scope.isolate, module);
        auto exports = Exports(scope, session);
        for (const auto* name : {"GameRules", "Console", "Instance"}) Export(scope, session, exports, name);
        session.ready = true;
    }
    catch (const std::exception& error)
    {
        session.module.Reset();
        throw std::runtime_error("Initialize host: " + Exception(scope, caught, error.what()));
    }
    return owned;
}

KHook::Return<void> Observe(const v8::FunctionCallbackInfo<v8::Value>&)
{
    if (currentSession && currentSession->id == observedSession && Engine().top() == currentSession->script)
        ++callbackEntries;
    return {KHook::Action::Ignore};
}
}

void Dispatch(ScriptContext& call)
{
    if (std::this_thread::get_id() != globals::gameThreadId) throw std::runtime_error("cs_script requires the game thread");
    // Slots 0..3: schema digest, 4: operation hash, 5: session, 6: method
    // hash, 7: entity handle. Remaining slots follow the generated signature.
    constexpr int HeaderSlots = 8;
    const int count = call.GetArgumentCount();
    if (count < HeaderSlots || count > ScriptContext::MaxArguments)
        throw std::runtime_error("Invalid cs_script argument count / transport version");
    for (int i = 0; i < 4; ++i)
        if (call.GetArgument<uint64_t>(i) != SchemaHash[i])
            throw std::runtime_error("Managed/native cs_script schema mismatch");
    const auto command = static_cast<Operation>(call.GetArgument<uint32_t>(4));
    switch (command)
    {
        case Operation::Current: case Operation::Host: case Operation::Ready:
        case Operation::Observe: case Operation::Observed: case Operation::Unobserve:
            if (count != HeaderSlots) throw std::runtime_error("Unexpected cs_script control arguments");
            break;
        case Operation::Invoke: break;
        default: throw std::runtime_error("Unknown cs_script operation hash");
    }
    if (!mapActive || !currentSession)
        throw std::runtime_error(initializationError.empty()
            ? "Shared cs_script context is not ready for this map"
            : "Shared cs_script initialization failed: " + initializationError);
    // Hold the session through reentrant callbacks. Old map/session IDs never
    // silently bind to a newly-created host.
    auto owned = currentSession;
    auto& session = *owned;
    if (command == Operation::Current)
    {
        Validate(session);
        call.SetResult(session.id);
        return;
    }
    const auto id = call.GetArgument<uint64_t>(5);
    if (id != session.id) throw std::runtime_error("Stale-map or deleted-host cs_script session; reacquire CsScriptRuntime.Shared");
    if (command == Operation::Host)
    {
        Validate(session);
        call.SetResult(session.host);
        return;
    }
    if (command == Operation::Observe)
    {
        Validate(session);
        if (observedSession) throw std::runtime_error("A callback observer is already installed");
        observation.AddFunction<void, const v8::FunctionCallbackInfo<v8::Value>&>(Engine().moneyCallback, Observe);
        observedSession = id;
        callbackEntries = 0;
        call.SetResult(uint64_t{0});
        return;
    }
    if (command == Operation::Observed)
    {
        call.SetResult(callbackEntries);
        return;
    }
    if (command == Operation::Unobserve)
    {
        if (observedSession == id) { observation.Clear(); observedSession = 0; }
        call.SetResult(uint64_t{0});
        return;
    }
    Scope scope(session);
    v8::TryCatch caught(scope.isolate);
    std::string memberName = "cs_script";
    try
    {
        auto bridge = Exports(scope, session);
        if (command == Operation::Ready)
        {
            for (const auto* name : {"GameRules", "Console", "Instance"}) Export(scope, session, bridge, name);
            call.SetResult(true);
            return;
        }
        if (!session.ready) throw std::runtime_error("cs_script session is not ready");
        const auto& descriptor = Description(call.GetArgument<uint32_t>(6));
        const auto* receiverName = descriptor.receiver;
        const auto* member = descriptor.member;
        memberName = std::string(receiverName) + "." + member;
        const int argc = count - HeaderSlots;
        if (argc < descriptor.minimumArguments || argc > descriptor.maximumArguments)
            throw std::runtime_error("Incorrect argument count");
        std::vector<Local> argv;
        argv.reserve(argc);
        for (int i = 0; i < argc; ++i)
        {
            const auto kind = descriptor.parameters[i];
            const int slot = HeaderSlots + i;
            if (kind == ValueKind::Number)
                argv.push_back(v8::Number::New(scope.isolate, call.GetArgument<double>(slot)));
            else if (kind == ValueKind::String)
            {
                const auto* text = call.GetArgument<const char*>(slot);
                if (!text) throw std::runtime_error("Null cs_script string argument");
                auto length = strnlen(text, 1024 * 1024 + 1);
                if (length > 1024 * 1024) throw std::runtime_error("cs_script string exceeds 1 MiB");
                argv.push_back(String(scope.isolate, std::string(text, length)));
            }
            else if (kind == ValueKind::Boolean)
            {
                auto value = call.GetArgument<uint64_t>(slot);
                if (value > 1) throw std::runtime_error("Invalid cs_script boolean argument");
                argv.push_back(v8::Boolean::New(scope.isolate, value != 0));
            }
            else throw std::runtime_error("Unsupported generated argument kind");
        }
        v8::Local<v8::Object> receiver;
        if (descriptor.entity)
        {
            auto wrapped = Engine().wrap(Entity(call.GetArgument<uint32_t>(7)));
            if (wrapped.IsEmpty() || !wrapped->IsObject()) throw std::runtime_error("Engine did not create an entity wrapper");
            receiver = wrapped.As<v8::Object>();
        }
        else receiver = Export(scope, session, bridge, receiverName);
        auto function = Get(scope, receiver, member);
        if (!function->IsFunction()) throw std::runtime_error("Member is not supported by this server build");
        Local result;
        if (!function.As<v8::Function>()->Call(scope.context, receiver, static_cast<int>(argv.size()), argv.data()).ToLocal(&result))
            throw std::runtime_error("V8 function invocation failed");
        if (result->IsPromise()) throw std::runtime_error("Promise results require an asynchronous lifetime contract");
        const auto kind = descriptor.result;
        if (kind == ValueKind::Void) { call.SetResult(uint64_t{0}); return; }
        if (kind == ValueKind::Number && result->IsNumber()) { call.SetResult(result.As<v8::Number>()->Value()); return; }
        if (kind == ValueKind::Boolean && result->IsBoolean()) { call.SetResult(result->IsTrue()); return; }
        if (kind == ValueKind::String && result->IsString())
        {
            auto text = Utf8(scope.isolate, result);
            if (text.size() > 1024 * 1024 || text.find('\0') != std::string::npos)
                throw std::runtime_error("String result exceeds 1 MiB or contains NUL");
            // ScriptContext owns a copy after the V8 handle scope exits.
            call.SetResult(text);
            return;
        }
        throw std::runtime_error("Script return value does not match its generated descriptor");
    }
    catch (const std::exception& error)
    {
        throw std::runtime_error(memberName + ": " + Exception(scope, caught, error.what()));
    }
}
void OnMapStart()
{
    OnMapEnd();
    mapActive = true;
    initializationPending = true;
    CSSHARP_CORE_INFO("Shared cs_script initialization scheduled after map simulation starts");
}

void OnGameFrame(bool simulating)
{
    if (!mapActive || !initializationPending || currentSession || !simulating) return;
    // Called from the GameFrame POST hook. StartupServer and PreWorldUpdate can
    // run while map systems are still starting, even with a current V8 isolate.
    // Let a full simulated frame finish, then initialize on a later frame.
    if (!completedSimulationFrame)
    {
        completedSimulationFrame = true;
        return;
    }
    if (!globals::entitySystem || !v8::Isolate::GetCurrent()) return;
    initializationPending = false; // Also prevents reentrant initialization.
    const auto generation = mapGeneration;
    CEntityInstance* host = nullptr;
    uint32_t handle = 0;
    try
    {
        CSSHARP_CORE_INFO("Shared cs_script initialization: validating engine adapter");
        auto& engine = Engine();
        CSSHARP_CORE_INFO("Shared cs_script initialization: creating point_script host");
        host = engine.createHost("point_script", -1);
        if (!host) throw std::runtime_error("Could not create the shared point_script host");
        handle = host->GetRefEHandle().ToInt();
        CSSHARP_CORE_INFO("Shared cs_script initialization: spawning host {}", handle);
        engine.spawnHost(host, nullptr);
        if (!mapActive || generation != mapGeneration) return;
        CSSHARP_CORE_INFO("Shared cs_script initialization: entering host context");
        auto session = InitializeHost(handle);
        if (!mapActive || generation != mapGeneration) return;
        Validate(*session);
        currentSession = std::move(session);
        initializationError.clear();
        CSSHARP_CORE_INFO("Shared cs_script context ready (session {}, host {})", currentSession->id, handle);
    }
    catch (const std::exception& error)
    {
        if (!mapActive || generation != mapGeneration) return;
        initializationError = error.what();
        CSSHARP_CORE_ERROR("Could not initialize shared cs_script context: {}", initializationError);
        if (host && globals::entitySystem &&
            globals::entitySystem->GetEntityInstance(CEntityHandle(handle)) == host)
            Engine().removeHost(host);
    }
}

void OnEntityDeleted(uint32_t handle)
{
    if (!currentSession || currentSession->host != handle) return;
    currentSession.reset();
    observation.Clear();
    observedSession = 0;
    // Round cleanup can remove dynamically-created entities. Recreate at the
    // a later simulated frame, never inside an entity-destruction callback.
    initializationPending = mapActive;
    completedSimulationFrame = false;
}
void OnMapEnd()
{
    mapActive = false;
    initializationPending = false;
    completedSimulationFrame = false;
    ++mapGeneration;
    initializationError.clear();
    currentSession.reset();
    observation.Clear();
    observedSession = 0;
}
void OnShutdown()
{
    CEntityInstance* host = nullptr;
    if (currentSession)
    {
        try { Validate(*currentSession); host = Entity(currentSession->host); }
        catch (const std::exception&) { /* The engine may already have removed it. */ }
    }
    OnMapEnd();
    if (host) Engine().removeHost(host);
}
}
#else
namespace counterstrikesharp::cs_script {
void Dispatch(ScriptContext&) { throw std::runtime_error("This build does not enable the experimental Linux cs_script bridge"); }
void OnMapStart() {}
void OnGameFrame(bool) {}
void OnShutdown() {}
void OnEntityDeleted(uint32_t) {}
void OnMapEnd() {}
}
#endif
