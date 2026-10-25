#include "core/cs_script_runtime.h"
#include "core/globals.h"
#include "scripting/script_engine.h"
#include <stdexcept>
#include <thread>

#include "core/gameconfig.h"
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

namespace counterstrikesharp::cs_script {
namespace {
using Local = v8::Local<v8::Value>;

struct Adapter
{
#ifdef _WIN32
    using GetContext = v8::Local<v8::Context>* (*)(void*, v8::Local<v8::Context>*);
#else
    using GetContext = v8::Local<v8::Context> (*)(void*);
#endif
    GetContext context;
    void (*push)(void*);
    void (*pop)();
    Local (*wrap)(CEntityInstance*);
    void* (*unwrap)(v8::Object*);
    void (*registerApi)(void*);
    CEntityInstance* (*createHost)(const char*, int);
    void (*spawnHost)(void*, void*);
    void (*removeHost)(CEntityInstance*);
    v8::Module::ResolveModuleCallback resolveModule;
    int scriptOffset, contextOffset, incarnationOffset, exportCountOffset, entityHandleOffset;

    Adapter()
    {
        auto& gamedata = *globals::gameConfig;
        auto resolve = [&gamedata](const char* name) {
            auto result = gamedata.ResolveSignature(name);
            if (!result) throw std::runtime_error(std::string("Missing engine signature: ") + name);
            return result;
        };
        context = reinterpret_cast<GetContext>(resolve("CsScript_GetContext"));
        push = reinterpret_cast<decltype(push)>(resolve("CsScript_Push"));
        pop = reinterpret_cast<decltype(pop)>(resolve("CsScript_Pop"));
        wrap = reinterpret_cast<decltype(wrap)>(resolve("CsScript_WrapEntity"));
        unwrap = reinterpret_cast<decltype(unwrap)>(resolve("CsScript_Unwrap"));
        registerApi = reinterpret_cast<decltype(registerApi)>(resolve("CsScript_RegisterApi"));
        resolveModule = reinterpret_cast<decltype(resolveModule)>(resolve("CsScript_ResolveModule"));

        auto offset = [&gamedata](const char* name) {
            auto result = gamedata.GetOffset(name);
            if (result < 0) throw std::runtime_error(std::string("Missing engine offset: ") + name);
            return result;
        };
        entityHandleOffset = offset("CsScript_EntityHandle");
        scriptOffset = offset("CsScript_HostScript");
        contextOffset = offset("CsScript_Context");
        incarnationOffset = offset("CsScript_Incarnation");
        exportCountOffset = offset("CsScript_ExportCount");
        createHost = reinterpret_cast<decltype(createHost)>(resolve("UTIL_CreateEntityByName"));
        spawnHost = reinterpret_cast<decltype(spawnHost)>(resolve("CBaseEntity_DispatchSpawn"));
        removeHost = reinterpret_cast<decltype(removeHost)>(resolve("UTIL_Remove"));
    }

    v8::Local<v8::Context> Context(void* script)
    {
#ifdef _WIN32
        v8::Local<v8::Context> result;
        context(script, &result);
        return result;
#else
        return context(script);
#endif
    }
};
Adapter& Engine()
{
    static Adapter engine;
    return engine;
}

struct HostContext
{
    uint32_t host;
    void* script;
    void* contextSlot;
    uint32_t incarnation;
    v8::Global<v8::Module> module;
    bool ready = false;
};
std::shared_ptr<HostContext> sharedHost;
bool mapActive = false, initializationPending = false;
bool completedSimulationFrame = false;
uint64_t mapGeneration = 0;
std::string initializationError;

CEntityInstance* Entity(uint32_t raw)
{
    if (!globals::entitySystem) throw std::runtime_error("Entity system is unavailable");
    CEntityHandle handle(raw);
    auto* entity = globals::entitySystem->GetEntityInstance(handle);
    if (!entity || entity->GetRefEHandle().ToInt() != raw) throw std::runtime_error("Deleted entity or stale entity serial");
    return entity;
}
v8::Local<v8::Object> WrapEntity(uint32_t raw)
{
    auto wrapped = Engine().wrap(Entity(raw));
    if (wrapped.IsEmpty() || !wrapped->IsObject()) throw std::runtime_error("Engine did not create an entity wrapper");
    return wrapped.As<v8::Object>();
}
uint32_t UnwrapEntity(v8::Local<v8::Object> object)
{
    auto* binding = static_cast<const char*>(Engine().unwrap(*object));
    uint32_t kind = 0, raw = 0;
    if (binding) std::memcpy(&kind, binding, sizeof(kind));
    if (kind != 2) throw std::runtime_error("Script result is not an engine entity wrapper");
    std::memcpy(&raw, binding + Engine().entityHandleOffset, sizeof(raw));

    if (!WrapEntity(raw)->StrictEquals(object)) throw std::runtime_error("Entity result belongs to a different script host");
    return raw;
}
bool IsEntityKind(ValueKind kind)
{
    return kind == ValueKind::Entity || kind == ValueKind::EntityOrUndefined || kind == ValueKind::EntityOrNull ||
           kind == ValueKind::EntityOrNullOrUndefined;
}
bool AllowsUndefined(ValueKind kind) { return kind == ValueKind::EntityOrUndefined || kind == ValueKind::EntityOrNullOrUndefined; }
bool AllowsNull(ValueKind kind) { return kind == ValueKind::EntityOrNull || kind == ValueKind::EntityOrNullOrUndefined; }
void Validate(const HostContext& hostContext)
{
    auto* host = Entity(hostContext.host);
    auto& engine = Engine();
    auto* script = reinterpret_cast<char*>(host) + engine.scriptOffset;
    if (script != hostContext.script || *reinterpret_cast<uint32_t*>(script + engine.incarnationOffset) != hostContext.incarnation ||
        *reinterpret_cast<void**>(script + engine.contextOffset) != hostContext.contextSlot)
        throw std::runtime_error("Shared cs_script host context was rebuilt");
}
struct V8Execution
{
    v8::Isolate* isolate;
    v8::Isolate::Scope isolateScope;
    v8::HandleScope handles;
    v8::Local<v8::Context> context;
    v8::Context::Scope contextScope;

    static v8::Isolate* GetIsolate(const HostContext& hostContext)
    {
        Validate(hostContext);

        // GetIsolate reads the persistent slot without allocating a local handle.
        if (!hostContext.contextSlot) throw std::runtime_error("Host has no V8 context");
        return reinterpret_cast<v8::Context*>(hostContext.contextSlot)->GetIsolate();
    }
    explicit V8Execution(const HostContext& hostContext)
        : isolate(GetIsolate(hostContext)), isolateScope(isolate), handles(isolate), context(Engine().Context(hostContext.script)),
          contextScope(context)
    {
        Engine().push(hostContext.script);
    }
    ~V8Execution() { Engine().pop(); }
    V8Execution(const V8Execution&) = delete;
};

v8::Local<v8::String> String(v8::Isolate* isolate, const std::string& text)
{
    v8::Local<v8::String> result;
    if (text.size() > 1024 * 1024 ||
        !v8::String::NewFromUtf8(isolate, text.data(), v8::NewStringType::kNormal, static_cast<int>(text.size())).ToLocal(&result))
        throw std::runtime_error("Cannot allocate cs_script string");
    return result;
}
std::string Utf8(v8::Isolate* isolate, Local value)
{
    v8::String::Utf8Value text(isolate, value);
    return *text ? std::string(*text, text.length()) : "<unprintable script exception>";
}
Local Get(V8Execution& scope, v8::Local<v8::Object> object, const std::string& key)
{
    Local value;
    if (!object->Get(scope.context, String(scope.isolate, key)).ToLocal(&value))
        throw std::runtime_error("Script property access failed: " + key);
    return value;
}
std::string Exception(V8Execution& scope, v8::TryCatch& caught, const std::string& fallback)
{
    if (!caught.HasCaught()) return fallback;
    std::string result = Utf8(scope.isolate, caught.Exception());
    Local stack;
    if (caught.StackTrace(scope.context).ToLocal(&stack) && stack->IsString()) result += "\n" + Utf8(scope.isolate, stack);
    return result;
}
v8::Local<v8::Object> Exports(V8Execution& scope, const HostContext& hostContext)
{
    if (hostContext.module.IsEmpty()) throw std::runtime_error("Host module is not initialized");
    auto module = hostContext.module.Get(scope.isolate);
    if (module->GetStatus() != v8::Module::kEvaluated) throw std::runtime_error("Host module is not evaluated");
    return module->GetModuleNamespace().As<v8::Object>();
}
v8::Local<v8::Object> Export(V8Execution& scope, const HostContext& hostContext, v8::Local<v8::Object> bridge, const std::string& name)
{
    auto value = Get(scope, bridge, name);
    if (!value->IsObject()) throw std::runtime_error("Missing module export: " + name);
    auto object = value.As<v8::Object>();
    auto* binding = static_cast<char*>(Engine().unwrap(*object));
    if (!binding || *reinterpret_cast<int*>(binding) != 1 || *reinterpret_cast<void**>(binding + 8) != hostContext.script)
        throw std::runtime_error("Module export belongs to a different script host: " + name);
    return object;
}
const MethodDescriptor& Description(uint32_t hash)
{
    static const auto methods = [] {
        std::unordered_map<uint32_t, const MethodDescriptor*> result;
        for (const auto& descriptor : Descriptors)
            if (!result.emplace(descriptor.id, &descriptor).second) throw std::runtime_error("Duplicate generated cs_script method hash");
        return result;
    }();
    auto it = methods.find(hash);
    if (it == methods.end()) throw std::runtime_error("Unknown cs_script method hash");
    return *it->second;
}

std::shared_ptr<HostContext> InitializeHost(uint32_t hostHandle)
{
    auto* host = Entity(hostHandle);
    auto& engine = Engine();
    auto* script = reinterpret_cast<char*>(host) + engine.scriptOffset;
    auto owned = std::make_shared<HostContext>();
    auto& hostContext = *owned;
    hostContext.host = hostHandle;
    hostContext.script = script;
    hostContext.contextSlot = *reinterpret_cast<void**>(script + engine.contextOffset);
    hostContext.incarnation = *reinterpret_cast<uint32_t*>(script + engine.incarnationOffset);
    V8Execution scope(hostContext);
    v8::TryCatch caught(scope.isolate);
    try
    {
        if (*reinterpret_cast<int*>(script + engine.exportCountOffset) != 0)
            throw std::runtime_error("Expected a fresh, empty point_script host");

        engine.registerApi(script);
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
        hostContext.module.Reset(scope.isolate, module);
        auto exports = Exports(scope, hostContext);
        for (const auto* name : { "GameRules", "Console", "Instance" })
            Export(scope, hostContext, exports, name);
        hostContext.ready = true;
    }
    catch (const std::exception& error)
    {
        hostContext.module.Reset();
        throw std::runtime_error("Initialize host: " + Exception(scope, caught, error.what()));
    }
    return owned;
}

void ValidateRequest(ScriptContext& call, int minimum, int maximum)
{
    if (std::this_thread::get_id() != globals::gameThreadId) throw std::runtime_error("cs_script requires the game thread");
    if (call.GetArgumentCount() < minimum || call.GetArgumentCount() > maximum)
        throw std::runtime_error("Invalid cs_script argument count");
}
std::shared_ptr<HostContext> CurrentHost()
{
    if (!mapActive || !sharedHost)
        throw std::runtime_error(initializationError.empty() ? "Shared cs_script context is not ready for this map"
                                                             : "Shared cs_script initialization failed: " + initializationError);
    Validate(*sharedHost);
    return sharedHost;
}
} // namespace

void GetHost(ScriptContext& call)
{
    ValidateRequest(call, 0, 0);
    call.SetResult(CurrentHost()->host);
}
void IsReady(ScriptContext& call)
{
    ValidateRequest(call, 0, 0);
    auto hostContext = CurrentHost();
    V8Execution scope(*hostContext);
    v8::TryCatch caught(scope.isolate);
    try
    {
        auto bridge = Exports(scope, *hostContext);
        for (const auto* name : { "GameRules", "Console", "Instance" })
            Export(scope, *hostContext, bridge, name);
        call.SetResult(hostContext->ready);
    }
    catch (const std::exception& error)
    {
        throw std::runtime_error("cs_script readiness: " + Exception(scope, caught, error.what()));
    }
}
void Invoke(ScriptContext& call)
{
    constexpr int HeaderSlots = 2;
    ValidateRequest(call, HeaderSlots, ScriptContext::MaxArguments);
    const int count = call.GetArgumentCount();

    // Retain the module if a reentrant call deletes the host.
    auto owned = CurrentHost();
    auto& hostContext = *owned;
    V8Execution scope(hostContext);
    v8::TryCatch caught(scope.isolate);
    std::string memberName = "cs_script";
    try
    {
        auto bridge = Exports(scope, hostContext);
        if (!hostContext.ready) throw std::runtime_error("Shared cs_script context is not ready");
        const auto& descriptor = Description(call.GetArgument<uint32_t>(0));
        const auto* receiverName = descriptor.receiver;
        const auto* member = descriptor.member;
        memberName = std::string(receiverName) + "." + member;
        const int argc = count - HeaderSlots;
        if (argc < descriptor.minimumArguments || argc > descriptor.maximumArguments) throw std::runtime_error("Incorrect argument count");
        std::vector<Local> argv;
        argv.reserve(argc);
        for (int i = 0; i < argc; ++i)
        {
            const auto kind = descriptor.parameters[i];
            const int slot = HeaderSlots + i;
            if (kind == ValueKind::Number) argv.push_back(v8::Number::New(scope.isolate, call.GetArgument<double>(slot)));
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
            else if (IsEntityKind(kind))
            {
                const auto slotValue = call.GetArgument<uint64_t>(slot);
                if (slotValue > (std::numeric_limits<uint32_t>::max)()) throw std::runtime_error("Invalid cs_script entity handle slot");
                const auto raw = static_cast<uint32_t>(slotValue);
                if (raw != (std::numeric_limits<uint32_t>::max)()) argv.push_back(WrapEntity(raw));
                else if (AllowsUndefined(kind))
                    argv.push_back(v8::Undefined(scope.isolate));
                else if (AllowsNull(kind))
                    argv.push_back(v8::Null(scope.isolate));
                else
                    throw std::runtime_error("Missing required cs_script entity argument");
            }
            else
                throw std::runtime_error("Unsupported generated argument kind");
        }
        v8::Local<v8::Object> receiver;
        if (descriptor.entity)
        {
            receiver = WrapEntity(call.GetArgument<uint32_t>(1));
        }
        else
            receiver = Export(scope, hostContext, bridge, receiverName);
        auto function = Get(scope, receiver, member);
        if (!function->IsFunction()) throw std::runtime_error("Member is not supported by this server build");
        Local result;
        if (!function.As<v8::Function>()->Call(scope.context, receiver, static_cast<int>(argv.size()), argv.data()).ToLocal(&result))
            throw std::runtime_error("V8 function invocation failed");
        if (result->IsPromise()) throw std::runtime_error("Promise results require an asynchronous lifetime contract");
        const auto kind = descriptor.result;
        if (IsEntityKind(kind))
        {
            if ((result->IsUndefined() && AllowsUndefined(kind)) || (result->IsNull() && AllowsNull(kind)))
            {
                call.SetResult((std::numeric_limits<uint32_t>::max)());
                return;
            }
            if (!result->IsObject()) throw std::runtime_error("Script return value does not match its entity descriptor");
            call.SetResult(UnwrapEntity(result.As<v8::Object>()));
            return;
        }
        if (kind == ValueKind::Void)
        {
            call.SetResult(uint64_t{ 0 });
            return;
        }
        if (kind == ValueKind::Number && result->IsNumber())
        {
            call.SetResult(result.As<v8::Number>()->Value());
            return;
        }
        if (kind == ValueKind::Boolean && result->IsBoolean())
        {
            call.SetResult(result->IsTrue());
            return;
        }
        if (kind == ValueKind::String && result->IsString())
        {
            auto text = Utf8(scope.isolate, result);
            if (text.size() > 1024 * 1024 || text.find('\0') != std::string::npos)
                throw std::runtime_error("String result exceeds 1 MiB or contains NUL");

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
}

void OnGameFrame(bool simulating)
{
    if (!mapActive || !initializationPending || sharedHost || !simulating) return;

    // Host creation must wait until the first simulated frame has completed.
    if (!completedSimulationFrame)
    {
        completedSimulationFrame = true;
        return;
    }
    if (!globals::entitySystem || !v8::Isolate::GetCurrent()) return;
    initializationPending = false;
    const auto generation = mapGeneration;
    CEntityInstance* host = nullptr;
    uint32_t handle = 0;
    try
    {
        auto& engine = Engine();
        host = engine.createHost("point_script", -1);
        if (!host) throw std::runtime_error("Could not create the shared point_script host");
        handle = host->GetRefEHandle().ToInt();
        engine.spawnHost(host, nullptr);
        if (!mapActive || generation != mapGeneration) return;
        auto hostContext = InitializeHost(handle);
        if (!mapActive || generation != mapGeneration) return;
        Validate(*hostContext);
        sharedHost = std::move(hostContext);
        initializationError.clear();
    }
    catch (const std::exception& error)
    {
        if (!mapActive || generation != mapGeneration) return;
        initializationError = error.what();
        CSSHARP_CORE_ERROR("Could not initialize shared cs_script context: {}", initializationError);
        if (host && globals::entitySystem && globals::entitySystem->GetEntityInstance(CEntityHandle(handle)) == host)
            Engine().removeHost(host);
    }
}

void OnEntityDeleted(uint32_t handle)
{
    if (!sharedHost || sharedHost->host != handle) return;
    sharedHost.reset();

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
    sharedHost.reset();
}
void OnShutdown()
{
    CEntityInstance* host = nullptr;
    if (sharedHost)
    {
        try
        {
            Validate(*sharedHost);
            host = Entity(sharedHost->host);
        }
        catch (const std::exception&)
        { /* Host already removed. */
        }
    }
    OnMapEnd();
    if (host) Engine().removeHost(host);
}
} // namespace counterstrikesharp::cs_script
