#include "dynamic_hook.h"
#include <dyncall/dyncallback/dyncall_callback.h>
#include <array>
#include <atomic>
#include <mutex>
#include <set>
#include <string>

namespace counterstrikesharp {
namespace {
// The currently supported public dynamic-function types are all scalar x64 ABI
// values. Variant/aggregate and void parameters must fail before installing a hook.
#define SCALAR_TYPES(X)                             \
    X(BOOL, bool, B, Char)                          \
    X(CHAR, char, c, Char)                          \
    X(UCHAR, unsigned char, C, UChar)               \
    X(SHORT, short, s, Short)                       \
    X(USHORT, unsigned short, S, UShort)            \
    X(INT, int, i, Int)                             \
    X(UINT, unsigned int, I, UInt)                  \
    X(LONG, long, j, Long)                          \
    X(ULONG, unsigned long, J, ULong)               \
    X(LONG_LONG, long long, l, LongLong)            \
    X(ULONG_LONG, unsigned long long, L, ULongLong) \
    X(FLOAT, float, f, Float)                       \
    X(DOUBLE, double, d, Double)                    \
    X(POINTER, void*, p, Pointer)                   \
    X(STRING, const char*, Z, Pointer)

char Signature(DataType_t type)
{
    switch (type)
    {
        case DATA_TYPE_VOID:
            return 'v';
#define SIG(E, T, F, N) \
    case DATA_TYPE_##E: \
        return #F[0];
            SCALAR_TYPES(SIG)
#undef SIG
        default:
            throw std::invalid_argument("Unsupported dynamic hook type");
    }
}
size_t TypeSize(DataType_t type)
{
    switch (type)
    {
        case DATA_TYPE_VOID:
            return 0;
#define SIZE(E, T, F, N) \
    case DATA_TYPE_##E:  \
        return sizeof(T);
            SCALAR_TYPES(SIZE)
#undef SIZE
        default:
            throw std::invalid_argument("Unsupported dynamic hook type");
    }
}

// Dyncall exposes signed call/argument functions for unsigned integers too.
// C++ bool uses one byte; dyncall DCbool is an int, so decode booleans via Char.
void Push(DCCallVM* vm, DataType_t type, const DCValue& v)
{
    switch (type)
    {
        case DATA_TYPE_BOOL:
            dcArgBool(vm, v.B);
            break;
        case DATA_TYPE_CHAR:
        case DATA_TYPE_UCHAR:
            dcArgChar(vm, v.c);
            break;
        case DATA_TYPE_SHORT:
        case DATA_TYPE_USHORT:
            dcArgShort(vm, v.s);
            break;
        case DATA_TYPE_INT:
        case DATA_TYPE_UINT:
            dcArgInt(vm, v.i);
            break;
        case DATA_TYPE_LONG:
        case DATA_TYPE_ULONG:
            dcArgLong(vm, v.j);
            break;
        case DATA_TYPE_LONG_LONG:
        case DATA_TYPE_ULONG_LONG:
            dcArgLongLong(vm, v.l);
            break;
        case DATA_TYPE_FLOAT:
            dcArgFloat(vm, v.f);
            break;
        case DATA_TYPE_DOUBLE:
            dcArgDouble(vm, v.d);
            break;
        case DATA_TYPE_POINTER:
        case DATA_TYPE_STRING:
            dcArgPointer(vm, v.p);
            break;
        default:
            throw std::invalid_argument("Unsupported dynamic argument");
    }
}
DCValue Call(void* address, const DynamicHookContext& frame)
{
    // A fresh VM is necessary: the target can recursively invoke another hook.
    auto vm = std::unique_ptr<DCCallVM, decltype(&dcFree)>(dcNewCallVM((frame.arguments.size() + 8) * 16), dcFree);
    if (!vm) throw std::bad_alloc();
    dcMode(vm.get(), DC_CALL_C_DEFAULT);
    for (size_t i = 0; i < frame.arguments.size(); ++i)
        Push(vm.get(), frame.types[i], frame.arguments[i]);
    DCValue result{};
    switch (frame.returnType)
    {
        case DATA_TYPE_VOID:
            dcCallVoid(vm.get(), address);
            break;
        case DATA_TYPE_BOOL:
            result.B = dcCallChar(vm.get(), address) != 0;
            break;
        case DATA_TYPE_CHAR:
        case DATA_TYPE_UCHAR:
            result.c = dcCallChar(vm.get(), address);
            break;
        case DATA_TYPE_SHORT:
        case DATA_TYPE_USHORT:
            result.s = dcCallShort(vm.get(), address);
            break;
        case DATA_TYPE_INT:
        case DATA_TYPE_UINT:
            result.i = dcCallInt(vm.get(), address);
            break;
        case DATA_TYPE_LONG:
        case DATA_TYPE_ULONG:
            result.j = dcCallLong(vm.get(), address);
            break;
        case DATA_TYPE_LONG_LONG:
        case DATA_TYPE_ULONG_LONG:
            result.l = dcCallLongLong(vm.get(), address);
            break;
        case DATA_TYPE_FLOAT:
            result.f = dcCallFloat(vm.get(), address);
            break;
        case DATA_TYPE_DOUBLE:
            result.d = dcCallDouble(vm.get(), address);
            break;
        case DATA_TYPE_POINTER:
        case DATA_TYPE_STRING:
            result.p = dcCallPointer(vm.get(), address);
            break;
        default:
            throw std::invalid_argument("Unsupported dynamic return type");
    }
    return result;
}

template <class T> void CopyValue(T* to, T* from) { new (to) T(*from); }
template <class T> void DeleteValue(T* value) { value->~T(); }

template <class T> void* SaveValue(T value, KHook::Action action, bool original, bool recall)
{
    auto init = reinterpret_cast<void*>(&CopyValue<T>);
    auto destroy = reinterpret_cast<void*>(&DeleteValue<T>);
    if (recall) return KHook::DoRecall(action, &value, sizeof(T), init, destroy);
    KHook::SaveReturnValue(action, &value, sizeof(T), init, destroy, original);
    return nullptr;
}
void* Save(DataType_t type, const DCValue& value, KHook::Action action, bool original = false, bool recall = false)
{
    switch (type)
    {
        case DATA_TYPE_VOID:
            if (recall) return KHook::DoRecall(action, nullptr, 0, nullptr, nullptr);
            KHook::SaveReturnValue(action, nullptr, 0, nullptr, nullptr, original);
            return nullptr;
#define SAVE(E, T, F, N) \
    case DATA_TYPE_##E:  \
        return SaveValue<T>(value.F, action, original, recall);
            SCALAR_TYPES(SAVE)
#undef SAVE
        default:
            throw std::invalid_argument("Unsupported dynamic return type");
    }
}
} // namespace

struct DynamicHook::State
{
    enum Phase
    {
        Pre,
        Post,
        Original,
        Return
    };
    struct Callback
    {
        State* state;
        Phase phase;
        DCCallback* closure = nullptr;
    };
    std::vector<DataType_t> types;
    DataType_t returnType;
    Handler handler;
    KHook::HookID_t id = KHook::INVALID_HOOK;
    std::array<Callback, 4> callbacks;
    std::atomic<bool> enabled{ true };
    std::atomic<bool> removed{ false };
    static std::vector<std::weak_ptr<State>>& Live()
    {
        static std::vector<std::weak_ptr<State>> live;
        return live;
    }
    static std::vector<std::shared_ptr<State>>& Retired()
    {
        static std::vector<std::shared_ptr<State>> retired;
        return retired;
    }
    static std::mutex& Mutex()
    {
        static std::mutex mutex;
        return mutex;
    }

    State(const std::vector<DataType_t>& types, DataType_t ret, Handler handler)
        : types(types), returnType(ret), handler(std::move(handler)),
          callbacks{ { { this, Pre }, { this, Post }, { this, Original }, { this, Return } } }
    {
    }
    ~State()
    {
        for (auto& cb : callbacks)
            if (cb.closure) dcbFreeCallback(cb.closure);
    }
    static void Removed(KHook::HookID_t) { KHook::GetContext<State>()->removed.store(true); }
    static char Invoke(DCCallback*, DCArgs* args, DCValue* result, void* userdata)
    {
        auto& cb = *static_cast<Callback*>(userdata);
        auto& state = *cb.state;
        *result = {};
        if (cb.phase == Return)
        {
            auto* value = KHook::GetCurrentValuePtr(true);
            if (state.returnType != DATA_TYPE_VOID && value) std::memcpy(result, value, TypeSize(state.returnType));
            KHook::DestroyReturnValue();
            return Signature(state.returnType);
        }
        DynamicHookContext frame(state.types, state.returnType);
        for (size_t i = 0; i < state.types.size(); ++i)
        {
            switch (state.types[i])
            {
#define READ(E, T, F, N)                           \
    case DATA_TYPE_##E:                            \
        frame.arguments[i].F = (T)dcbArg##N(args); \
        break;
                SCALAR_TYPES(READ)
#undef READ
                default:
                    break; // constructor validates types
            }
        }
        if (cb.phase == Original)
        {
            *result = Call(KHook::GetOriginalFunction(), frame);
            Save(state.returnType, *result, KHook::Action::Ignore, true);
        }
        else
        {
            if (cb.phase == Post)
            {
                auto* value = KHook::GetCurrentValuePtr();
                if (state.returnType != DATA_TYPE_VOID && value) std::memcpy(&frame.result, value, TypeSize(state.returnType));
            }
            auto action = state.enabled.load() ? state.handler(cb.phase == Post, frame) : KHook::Action::Ignore;
            // A return write is independent of argument writes. Changed arguments
            // must reach subsequent hooks, so resume the chain via Recall.
            if (frame.returnChanged && action == KHook::Action::Ignore) action = KHook::Action::Override;
            if (frame.argumentsChanged && cb.phase == Pre)
            {
                auto* next = Save(state.returnType, frame.result, action, false, true);
                Call(next, frame);
            }
            else
                Save(state.returnType, frame.result, action);
        }
        return Signature(state.returnType);
    }
};

DynamicHook::DynamicHook(void* address, const std::vector<DataType_t>& types, DataType_t returnType, Handler handler)
    : m_state(std::make_shared<State>(types, returnType, std::move(handler)))
{
    if (!address) throw std::invalid_argument("Cannot hook a null function");
    std::string signature;
    for (auto type : types)
    {
        if (type == DATA_TYPE_VOID) throw std::invalid_argument("Void is not a parameter type");
        signature += Signature(type);
    }
    signature += ')';
    signature += Signature(returnType);
    for (auto& cb : m_state->callbacks)
    {
        cb.closure = dcbNewCallback(signature.c_str(), &State::Invoke, &cb);
        if (!cb.closure) throw std::runtime_error("Could not allocate dynamic hook callback");
    }
#ifdef _WIN32
    // x64 home space plus stack arguments, all supported types occupy one slot.
    unsigned stackSize = 32 + (types.size() > 4 ? (types.size() - 4) * 8 : 0);
#else
    unsigned stackSize = types.size() * 8;
#endif
    auto& cb = m_state->callbacks;
    m_state->id =
        KHook::SetupHook(address, m_state.get(), reinterpret_cast<void*>(&State::Removed), reinterpret_cast<void*>(cb[State::Pre].closure),
                         reinterpret_cast<void*>(cb[State::Post].closure), reinterpret_cast<void*>(cb[State::Return].closure),
                         reinterpret_cast<void*>(cb[State::Original].closure), stackSize, true);
    if (m_state->id == KHook::INVALID_HOOK) throw std::runtime_error("KHook could not install dynamic hook");
    std::lock_guard lock(State::Mutex());
    State::Live().push_back(m_state);
}

DynamicHook::~DynamicHook()
{
    if (m_state && !m_state->removed.load())
    {
        // Disable dispatch immediately, but keep executing closures alive until
        // KHook acknowledges asynchronous removal. No deletion callback remains
        // queued in Metamod after our synchronous unload barrier.
        m_state->enabled.store(false);
        {
            std::lock_guard lock(State::Mutex());
            State::Retired().push_back(m_state);
        }
        KHook::RemoveHook(m_state->id, true);
    }
}

void DynamicHook::CollectRetired()
{
    std::lock_guard lock(State::Mutex());
    auto& retired = State::Retired();
    retired.erase(std::remove_if(retired.begin(), retired.end(),
                                 [](const auto& state) {
        return state->removed.load();
    }),
                  retired.end());
    auto& live = State::Live();
    live.erase(std::remove_if(live.begin(), live.end(),
                              [](const auto& state) {
        return state.expired();
    }),
               live.end());
}

void DynamicHook::ShutdownAll()
{
    std::vector<std::shared_ptr<State>> states;
    {
        std::lock_guard lock(State::Mutex());
        for (auto& weak : State::Live())
            if (auto state = weak.lock()) states.push_back(std::move(state));
    }
    for (auto& state : states)
    {
        state->enabled.store(false);
        // Use the original ID even if Removed already ran. This also waits for
        // an in-progress asynchronous removal to leave Metamod's registry lock.
        KHook::RemoveHook(state->id, false);
    }
    CollectRetired();
}
} // namespace counterstrikesharp
