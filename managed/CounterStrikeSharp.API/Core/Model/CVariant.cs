using System.Reflection.Metadata;

namespace CounterStrikeSharp.API.Core;

// Taken directly from datamap.h in hl2sdk.
public enum VariantFieldTypes : uint
{
    FIELD_VOID = 0,         // No type or value
    FIELD_FLOAT,            // Any floating point value
    FIELD_STRING,           // A string ID (return from ALLOC_STRING)
    FIELD_VECTOR,           // Any vector, QAngle, or AngularImpulse
    FIELD_QUATERNION,       // A quaternion
    FIELD_INTEGER,          // Any integer or enum
    FIELD_BOOLEAN,          // boolean, implemented as an int, I may use this as a hint for compression
    FIELD_SHORT,            // 2 byte integer
    FIELD_CHARACTER,        // a byte
    FIELD_COLOR32,          // 8-bit per channel r,g,b,a (32bit color)
    FIELD_EMBEDDED,         // an embedded object with a datadesc, recursively traverse and embedded class/structure based on an additional typedescription
    FIELD_CUSTOM,           // special type that contains function pointers to it's read/write/parse functions

    FIELD_CLASSPTR,         // CBaseEntity *
    FIELD_EHANDLE,          // Entity handle

    FIELD_POSITION_VECTOR,  // A world coordinate (these are fixed up across level transitions automagically)
    FIELD_TIME,             // a floating point time (these are fixed up automatically too!)
    FIELD_TICK,             // an integer tick count( fixed up similarly to time)
    FIELD_SOUNDNAME,        // Engine string that is a sound name (needs precache)

    FIELD_INPUT,            // a list of inputed data fields (all derived from CMultiInputVar)
    FIELD_FUNCTION,         // A class function pointer (Think, Use, etc)

    FIELD_VMATRIX,          // a vmatrix (output coords are NOT worldspace)

    // NOTE: Use float arrays for local transformations that don't need to be fixed up.
    FIELD_VMATRIX_WORLDSPACE,// A VMatrix that maps some local space to world space (translation is fixed up on level transitions)
    FIELD_MATRIX3X4_WORLDSPACE, // matrix3x4_t that maps some local space to world space (translation is fixed up on level transitions)

    FIELD_INTERVAL,         // a start and range floating point interval ( e.g., 3.2->3.6 == 3.2 and 0.4 )
    FIELD_UNUSED,

    FIELD_VECTOR2D,         // 2 floats
    FIELD_INTEGER64,        // 64bit integer

    FIELD_VECTOR4D,         // 4 floats

    FIELD_RESOURCE,

    FIELD_TYPEUNKNOWN,

    FIELD_CSTRING,
    FIELD_HSCRIPT,
    FIELD_VARIANT,
    FIELD_UINT64,
    FIELD_FLOAT64,
    FIELD_POSITIVEINTEGER_OR_NULL,
    FIELD_HSCRIPT_NEW_INSTANCE,
    FIELD_UINT,
    FIELD_UTLSTRINGTOKEN,
    FIELD_QANGLE,
    FIELD_NETWORK_ORIGIN_CELL_QUANTIZED_VECTOR,
    FIELD_HMATERIAL,
    FIELD_HMODEL,
    FIELD_NETWORK_QUANTIZED_VECTOR,
    FIELD_NETWORK_QUANTIZED_FLOAT,
    FIELD_DIRECTION_VECTOR_WORLDSPACE,
    FIELD_QANGLE_WORLDSPACE,
    FIELD_QUATERNION_WORLDSPACE,
    FIELD_HSCRIPT_LIGHTBINDING,
    FIELD_V8_VALUE,
    FIELD_V8_OBJECT,
    FIELD_V8_ARRAY,
    FIELD_V8_CALLBACK_INFO,
    FIELD_UTLSTRING,

    FIELD_NETWORK_ORIGIN_CELL_QUANTIZED_POSITION_VECTOR,
    FIELD_HRENDERTEXTURE,

    FIELD_HPARTICLESYSTEMDEFINITION,
    FIELD_UINT8,
    FIELD_UINT16,
    FIELD_CTRANSFORM,
    FIELD_CTRANSFORM_WORLDSPACE,
    FIELD_HPOSTPROCESSING,
    FIELD_MATRIX3X4,
    FIELD_SHIM,
    FIELD_CMOTIONTRANSFORM,
    FIELD_CMOTIONTRANSFORM_WORLDSPACE,
    FIELD_ATTACHMENT_HANDLE,
    FIELD_AMMO_INDEX,
    FIELD_CONDITION_ID,
    FIELD_AI_SCHEDULE_BITS,
    FIELD_MODIFIER_HANDLE,
    FIELD_ROTATION_VECTOR,
    FIELD_ROTATION_VECTOR_WORLDSPACE,
    FIELD_HVDATA,
    FIELD_SCALE32,
    FIELD_STRING_AND_TOKEN,
    FIELD_ENGINE_TIME,
    FIELD_ENGINE_TICK,
    FIELD_WORLD_GROUP_ID,
    FIELD_GLOBALSYMBOL,

    FIELD_TYPECOUNT
}

public class CVariant : NativeObject
{
    public CVariant(IntPtr pointer) : base(pointer)
    {
    }

    public bool IsValid => Handle != IntPtr.Zero;

    public VariantFieldTypes FieldType => (VariantFieldTypes)NativeAPI.GetVariantType(Handle);

    public T Get<T>()
    {
        Type typeFromHandle = typeof(T);
        object obj;
        if (typeFromHandle == typeof(float))
        {
            obj = NativeAPI.GetVariantFloat(Handle);
        }
        else if (typeFromHandle == typeof(int))
        {
            obj = NativeAPI.GetVariantInt(Handle);
        }
        else if (typeFromHandle == typeof(string))
        {
            obj = NativeAPI.GetVariantString(Handle);
        }
        else if (typeFromHandle == typeof(bool))
        {
            obj = NativeAPI.GetVariantBool(Handle);
        }
        else if (typeFromHandle == typeof(uint))
        {
            obj = NativeAPI.GetVariantUint(Handle);
        }
        else
        {
            throw new NotSupportedException();
        }

        return (T)obj;
    }
}
