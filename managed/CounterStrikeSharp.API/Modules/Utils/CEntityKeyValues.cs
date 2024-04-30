using System.Drawing;
using System.Numerics;

namespace CounterStrikeSharp.API.Modules.Utils
{
    /// <summary>
    /// EntityKeyValues
    /// WARNING: This is intended to only use with DispatchSpawn for now!
    /// TODO: Might need more works with vector and angle classes, because this is only managed side, no need to alloc memory on unmanaged side, but still need to compatible to original vector classes
    /// </summary>
    public class CEntityKeyValues
    {
        internal Dictionary<string, KeyValueContainer> keyValues = new();

        // Getter
        public bool GetBool(string key, bool defaultValue = false) => GetValue(key, defaultValue);
        public int GetInt(string key, int defaultValue = 0) => GetValue(key, defaultValue);
        public uint GetUInt(string key, uint defaultValue = 0) => GetValue(key, defaultValue);
        public long GetInt64(string key, long defaultValue = 0) => GetValue(key, defaultValue);
        public ulong GetUInt64(string key, ulong defaultValue = 0) => GetValue(key, defaultValue);
        public float GetFloat(string key, float defaultValue = 0) => GetValue(key, defaultValue);
        public double GetDouble(string key, double defaultValue = 0) => GetValue(key, defaultValue);
        public string GetString(string key, string defaultValue = "") => GetValue(key, defaultValue);
        public nint GetPointer(string key, nint defaultValue = 0) => GetValue(key, defaultValue);
        public uint GetStringToken(string key, uint defaultValue = 0) => GetValue(key, defaultValue);
        public CEntityHandle? GetEHandle(string key, CEntityHandle? defaultValue = null) => GetValue(key, defaultValue);
        public Color GetColor(string key) => GetValue(key, Color.Empty);
        public Vector3? GetVector(string key, Vector3? defaultValue = null) => GetValue(key, defaultValue);
        public Vector2? GetVector2D(string key, Vector2? defaultValue = null) => GetValue(key, defaultValue);
        public Vector4? GetVector4D(string key, Vector4? defaultValue = null) => GetValue(key, defaultValue);
        public Vector4? GetQuaternion(string key, Vector4? defaultValue = null) => GetValue(key, defaultValue);
        public QAngle? GetAngle(string key, QAngle? defaultValue = null) => GetValue(key, defaultValue);
        public Matrix3x4? GetMatrix3x4(string key, Matrix3x4? defaultValue = null) => GetValue(key, defaultValue);

        // Setter
        public void SetBool(string key, bool value) => SetValue<bool>(key, KeyValuesType.TYPE_BOOL, value);
        public void SetInt(string key, int value) => SetValue<int>(key, KeyValuesType.TYPE_INT, value);
        public void SetUInt(string key, uint value) => SetValue<uint>(key, KeyValuesType.TYPE_UINT, value);
        public void SetInt64(string key, long value) => SetValue<long>(key, KeyValuesType.TYPE_INT64, value);
        public void SetUInt64(string key, ulong value) => SetValue<ulong>(key, KeyValuesType.TYPE_UINT64, value);
        public void SetFloat(string key, float value) => SetValue<float>(key, KeyValuesType.TYPE_FLOAT, value);
        public void SetDouble(string key, double value) => SetValue<double>(key, KeyValuesType.TYPE_DOUBLE, value);
        public void SetString(string key, string value) => SetValue<string>(key, KeyValuesType.TYPE_STRING, value);
        public void SetPointer(string key, nint value) => SetValue<nint>(key, KeyValuesType.TYPE_POINTER, value);
        public void SetStringToken(string key, uint value) => SetValue<uint>(key, KeyValuesType.TYPE_STRING_TOKEN, value); // Essentially is integer
        public void SetEHandle(string key, CEntityHandle value) => SetValue<CEntityHandle>(key, KeyValuesType.TYPE_EHANDLE, value);
        public void SetColor(string key, Color value) => SetValue<Color>(key, KeyValuesType.TYPE_COLOR, value);
        public void SetVector(string key, Vector3 value) => SetValue<Vector3>(key, KeyValuesType.TYPE_VECTOR, value);
        public void SetVector2D(string key, Vector2 value) => SetValue<Vector2>(key, KeyValuesType.TYPE_VECTOR2D, value);
        public void SetVector4D(string key, Vector4 value) => SetValue<Vector4>(key, KeyValuesType.TYPE_VECTOR4D, value);
        public void SetQuaternion(string key, Vector4 value) => SetValue<Vector4>(key, KeyValuesType.TYPE_QUATERNION, value); // Same class with Vector4D
        public void SetAngle(string key, QAngle value) => SetValue<Angle>(key, KeyValuesType.TYPE_QANGLE, value);
        public void SetMatrix3x4(string key, Matrix3x4 value) => SetValue<Matrix3x4>(key, KeyValuesType.TYPE_MATRIX3X4, value);

        public bool Remove(string key) => keyValues.Remove(key);
        public void Clear() => keyValues.Clear();
        public int Count => keyValues.Count;

        internal void SetValue<T>(string key, KeyValuesType type, object value)
        {
            if (value == null)
                throw new ArgumentNullException("Value can't be null!");

            if (keyValues.TryGetValue(key, out KeyValueContainer? v))
                v.Set(value);
            else
            {
                KeyValueContainer container = new(type, value);
                keyValues.Add(key, container);
            }
        }

        internal T GetValue<T>(string key, T defaultValue)
        {
            if (keyValues.TryGetValue(key, out KeyValueContainer? v))
                return v.Get<T>();

            return defaultValue;
        }

        // Build keyvalues and send to C++ side
        internal int Build(out object[] list)
        {
            if (keyValues.Count == 0)
            {
                list = Array.Empty<object>();
                return 0;
            }

            List<object> valueLists = new();

            foreach (var kv in keyValues)
            {
                valueLists.Add(kv.Key);
                KeyValuesType _type = kv.Value.GetContainerType();
                valueLists.Add(_type);
                switch (_type)
                {
                    // Special type handle
                    case KeyValuesType.TYPE_EHANDLE:
                        valueLists.Add(kv.Value.Get<CounterStrikeSharp.API.Modules.Utils.CEntityHandle>().Raw);
                        break;

                    case KeyValuesType.TYPE_COLOR:
                        Color color = kv.Value.Get<Color>();
                        valueLists.Add(color.R);
                        valueLists.Add(color.G);
                        valueLists.Add(color.B);
                        valueLists.Add(color.A);
                        break;

                    case KeyValuesType.TYPE_VECTOR:
                        Vector3 vec = kv.Value.Get<Vector3>();
                        valueLists.Add(vec.X);
                        valueLists.Add(vec.Y);
                        valueLists.Add(vec.Z);
                        break;

                    case KeyValuesType.TYPE_VECTOR2D:
                        Vector2 vec2D = kv.Value.Get<Vector2>();
                        valueLists.Add(vec2D.X);
                        valueLists.Add(vec2D.Y);
                        break;

                    // Same thing, idk why valve seperate this
                    case KeyValuesType.TYPE_VECTOR4D:
                    case KeyValuesType.TYPE_QUATERNION:
                        Vector4 vec4D = kv.Value.Get<Vector4>();
                        valueLists.Add(vec4D.X);
                        valueLists.Add(vec4D.Y);
                        valueLists.Add(vec4D.Z);
                        valueLists.Add(vec4D.W);
                        break;

                    case KeyValuesType.TYPE_QANGLE:
                        Angle qAng = kv.Value.Get<Angle>();
                        valueLists.Add(qAng.X);
                        valueLists.Add(qAng.Y);
                        valueLists.Add(qAng.Z);
                        break;

                    case KeyValuesType.TYPE_MATRIX3X4:
                        Matrix3x4 matrix = kv.Value.Get<Matrix3x4>();
                        valueLists.Add(matrix.M11);
                        valueLists.Add(matrix.M12);
                        valueLists.Add(matrix.M13);
                        valueLists.Add(matrix.M14);
                        valueLists.Add(matrix.M21);
                        valueLists.Add(matrix.M22);
                        valueLists.Add(matrix.M23);
                        valueLists.Add(matrix.M24);
                        valueLists.Add(matrix.M31);
                        valueLists.Add(matrix.M32);
                        valueLists.Add(matrix.M33);
                        valueLists.Add(matrix.M34);
                        break;

                    // C# default type handle
                    default:
                        valueLists.Add(kv.Value.Get<object>());
                        break;
                }
            }

            list = valueLists.ToArray();

            return keyValues.Count;
        }

        internal class KeyValueContainer
        {
            private KeyValuesType type;
            private object value;

            public KeyValueContainer(KeyValuesType type, object value)
            {
                this.type = type;
                this.value = value;
            }

            public KeyValuesType GetContainerType() => type;
            public T Get<T>() => (T)value;

            // Shut visual studio up
            #pragma warning disable 8601
            public void Set<T>(T val) => value = val;
            #pragma warning restore
        }

        internal enum KeyValuesType : uint
        {
            TYPE_BOOL,
            TYPE_INT,
            TYPE_UINT,
            TYPE_INT64,
            TYPE_UINT64,
            TYPE_FLOAT,
            TYPE_DOUBLE,
            TYPE_STRING,
            TYPE_POINTER,
            TYPE_STRING_TOKEN,
            TYPE_EHANDLE,
            TYPE_COLOR,
            TYPE_VECTOR,
            TYPE_VECTOR2D,
            TYPE_VECTOR4D,
            TYPE_QUATERNION,
            TYPE_QANGLE,
            TYPE_MATRIX3X4
        }
    }
}
