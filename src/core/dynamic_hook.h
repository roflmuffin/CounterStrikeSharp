#pragma once

#include <khook.hpp>
#include <dyncall/dyncall/dyncall_value.h>
#include <cstring>
#include <functional>
#include <memory>
#include <stdexcept>
#include <vector>

namespace counterstrikesharp {

enum DataType_t
{
    DATA_TYPE_VOID,
    DATA_TYPE_BOOL,
    DATA_TYPE_CHAR,
    DATA_TYPE_UCHAR,
    DATA_TYPE_SHORT,
    DATA_TYPE_USHORT,
    DATA_TYPE_INT,
    DATA_TYPE_UINT,
    DATA_TYPE_LONG,
    DATA_TYPE_ULONG,
    DATA_TYPE_LONG_LONG,
    DATA_TYPE_ULONG_LONG,
    DATA_TYPE_FLOAT,
    DATA_TYPE_DOUBLE,
    DATA_TYPE_POINTER,
    DATA_TYPE_STRING,
    DATA_TYPE_VARIANT
};

// An invocation-local handle exposed to managed DynamicHook. Arguments are
// decoded from the platform ABI by dyncallback, not from a private detour.
class DynamicHookContext
{
  public:
    DynamicHookContext(const std::vector<DataType_t>& types, DataType_t returnType)
        : types(types), returnType(returnType), arguments(types.size())
    {
    }

    template <class T> T getArgument(int index) const { return Read<T>(arguments.at(index)); }
    template <class T> void setArgument(int index, T value)
    {
        Store(arguments.at(index), types.at(index), value);
        argumentsChanged = true;
    }
    template <class T> T getReturnValue() const { return Read<T>(result); }
    template <class T> void setReturnValue(T value)
    {
        Store(result, returnType, value);
        returnChanged = true;
    }

    const std::vector<DataType_t>& types;
    DataType_t returnType;
    std::vector<DCValue> arguments;
    DCValue result{};
    bool argumentsChanged = false;
    bool returnChanged = false;

  private:
    template <class T> static T Read(const DCValue& value)
    {
        static_assert(sizeof(T) <= sizeof(DCValue));
        T result;
        std::memcpy(&result, &value, sizeof(T));
        return result;
    }
    template <class T> static void Store(DCValue& target, DataType_t type, T value)
    {
        target = {};
        if constexpr (std::is_pointer_v<T>)
        {
            if (type != DATA_TYPE_POINTER && type != DATA_TYPE_STRING) throw std::invalid_argument("Hook value is not a pointer");
            target.p = const_cast<void*>(static_cast<const void*>(value));
        }
        else
        {
            switch (type)
            {
                case DATA_TYPE_BOOL:
                    target.B = static_cast<bool>(value);
                    break;
                case DATA_TYPE_CHAR:
                    target.c = static_cast<char>(value);
                    break;
                case DATA_TYPE_UCHAR:
                    target.C = static_cast<unsigned char>(value);
                    break;
                case DATA_TYPE_SHORT:
                    target.s = static_cast<short>(value);
                    break;
                case DATA_TYPE_USHORT:
                    target.S = static_cast<unsigned short>(value);
                    break;
                case DATA_TYPE_INT:
                    target.i = static_cast<int>(value);
                    break;
                case DATA_TYPE_UINT:
                    target.I = static_cast<unsigned int>(value);
                    break;
                case DATA_TYPE_LONG:
                    target.j = static_cast<long>(value);
                    break;
                case DATA_TYPE_ULONG:
                    target.J = static_cast<unsigned long>(value);
                    break;
                case DATA_TYPE_LONG_LONG:
                    target.l = static_cast<long long>(value);
                    break;
                case DATA_TYPE_ULONG_LONG:
                    target.L = static_cast<unsigned long long>(value);
                    break;
                case DATA_TYPE_FLOAT:
                    target.f = static_cast<float>(value);
                    break;
                case DATA_TYPE_DOUBLE:
                    target.d = static_cast<double>(value);
                    break;
                default:
                    throw std::invalid_argument("Hook value is not numeric");
            }
        }
    }
};

class DynamicHook
{
  public:
    using Handler = std::function<KHook::Action(bool post, DynamicHookContext&)>;
    DynamicHook(void* address, const std::vector<DataType_t>& types, DataType_t returnType, Handler handler);
    ~DynamicHook();
    DynamicHook(const DynamicHook&) = delete;
    DynamicHook& operator=(const DynamicHook&) = delete;
    static void CollectRetired();
    static void ShutdownAll(); // synchronous; call outside hook callbacks

  private:
    struct State;
    std::shared_ptr<State> m_state;
};
} // namespace counterstrikesharp
