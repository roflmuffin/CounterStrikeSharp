/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System.Drawing;
using System.Numerics;

// TODO: change from System.Numerics to exposed native values (Vector2D, Vector4D, Quaternion)

namespace CounterStrikeSharp.API.Modules.Utils
{
    /// <summary>
    /// <b>WARNING: This is intended to be only used with <see cref="CBaseEntity.DispatchSpawn"/> for now!</b>
    /// </summary>
    public class CEntityKeyValues : NativeObject
    {
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

        public CEntityKeyValues() : base(NativeAPI.EntityKeyValuesNew())
            { }

        public CEntityKeyValues(nint pointer) : base(pointer)
            { }

        #region GETTER
        public bool GetBool(string key, bool defaultValue = false) => GetValue<bool>(key, KeyValuesType.TYPE_BOOL, defaultValue);

        public int GetInt(string key, int defaultValue = 0) => GetValue<int>(key, KeyValuesType.TYPE_INT, defaultValue);

        public uint GetUInt(string key, uint defaultValue = 0) => GetValue<uint>(key, KeyValuesType.TYPE_UINT, defaultValue);

        public long GetInt64(string key, long defaultValue = 0) => GetValue<long>(key, KeyValuesType.TYPE_INT64, defaultValue);

        public ulong GetUInt64(string key, ulong defaultValue = 0) => GetValue<ulong>(key, KeyValuesType.TYPE_UINT64, defaultValue);

        public float GetFloat(string key, float defaultValue = 0) => GetValue<float>(key, KeyValuesType.TYPE_FLOAT, defaultValue);

        public double GetDouble(string key, double defaultValue = 0) => GetValue<double>(key, KeyValuesType.TYPE_DOUBLE, defaultValue);

        public string GetString(string key, string defaultValue = "") => GetValue<string>(key, KeyValuesType.TYPE_STRING, defaultValue);

        public nint GetPointer(string key, nint defaultValue = 0) => GetValue<nint>(key, KeyValuesType.TYPE_POINTER, defaultValue);

        public uint GetStringToken(string key, uint defaultValue = 0) => GetValue<uint>(key, KeyValuesType.TYPE_STRING_TOKEN, defaultValue);

        public CEntityHandle? GetEHandle(string key, CEntityHandle? defaultValue = null) => GetValue<CEntityHandle?>(key, KeyValuesType.TYPE_EHANDLE, defaultValue);

        public Color GetColor(string key) => GetValue<Color>(key, KeyValuesType.TYPE_COLOR, Color.Empty);

        public Vector? GetVector(string key, Vector? defaultValue = null) => GetValue<Vector?>(key, KeyValuesType.TYPE_VECTOR, defaultValue);

        public Vector2? GetVector2D(string key, Vector2? defaultValue = null) => GetValue<Vector2?>(key, KeyValuesType.TYPE_VECTOR2D, defaultValue);

        public Vector4? GetVector4D(string key, Vector4? defaultValue = null) => GetValue<Vector4?>(key, KeyValuesType.TYPE_VECTOR4D, defaultValue);

        public Vector4? GetQuaternion(string key, Vector4? defaultValue = null) => GetValue<Vector4?>(key, KeyValuesType.TYPE_QUATERNION, defaultValue);

        public QAngle? GetAngle(string key, QAngle? defaultValue = null) => GetValue<QAngle?>(key, KeyValuesType.TYPE_QANGLE, defaultValue);

        public Matrix3x4? GetMatrix3x4(string key, Matrix3x4? defaultValue = null) => GetValue<Matrix3x4?>(key, KeyValuesType.TYPE_MATRIX3X4, defaultValue);
#endregion

#region SETTER
        public void SetBool(string key, bool value) => SetValue<bool>(key, KeyValuesType.TYPE_BOOL, value);

        public void SetInt(string key, int value) => SetValue<int>(key, KeyValuesType.TYPE_INT, value);

        public void SetUInt(string key, uint value) => SetValue<uint>(key, KeyValuesType.TYPE_UINT, value);

        public void SetInt64(string key, long value) => SetValue<long>(key, KeyValuesType.TYPE_INT64, value);

        public void SetUInt64(string key, ulong value) => SetValue<ulong>(key, KeyValuesType.TYPE_UINT64, value);

        public void SetFloat(string key, float value) => SetValue<float>(key, KeyValuesType.TYPE_FLOAT, value);

        public void SetDouble(string key, double value) => SetValue<double>(key, KeyValuesType.TYPE_DOUBLE, value);

        public void SetString(string key, string value) => SetValue<string>(key, KeyValuesType.TYPE_STRING, value);

        public void SetPointer(string key, nint value) => SetValue<nint>(key, KeyValuesType.TYPE_POINTER, value);

        public void SetStringToken(string key, uint value) => SetValue<uint>(key, KeyValuesType.TYPE_STRING_TOKEN, value);

        public void SetEHandle(string key, CEntityHandle value) => SetValue<CEntityHandle>(key, KeyValuesType.TYPE_EHANDLE, value);

        public void SetColor(string key, Color value) => SetValue<Color>(key, KeyValuesType.TYPE_COLOR, value);

        public void SetVector(string key, float x, float y, float z) => SetValue<Vector3>(key, KeyValuesType.TYPE_VECTOR, new Vector3(x, y, z));

        public void SetVector2D(string key, float x, float y) => SetValue<Vector2>(key, KeyValuesType.TYPE_VECTOR2D, new Vector2(x, y));

        public void SetVector2D(string key, Vector2 value) => SetValue<Vector2>(key, KeyValuesType.TYPE_VECTOR2D, value);

        public void SetVector4D(string key, float x, float y, float z, float w) => SetValue<Vector4>(key, KeyValuesType.TYPE_VECTOR4D, new Vector4(x, y, z, w));

        public void SetVector4D(string key, Vector4 value) => SetValue<Vector4>(key, KeyValuesType.TYPE_VECTOR4D, value);

        public void SetQuaternion(string key, float x, float y, float z, float w) => SetValue<Vector4>(key, KeyValuesType.TYPE_QUATERNION, new Vector4(x, y, z, w));

        public void SetQuaternion(string key, Vector4 value) => SetValue<Vector4>(key, KeyValuesType.TYPE_QUATERNION, value);

        public void SetAngle(string key, float pitch, float yaw, float roll) => SetValue<QAngle>(key, KeyValuesType.TYPE_QANGLE, new QAngle(pitch, yaw, roll));

        public void SetAngle(string key, QAngle angle) => SetValue<QAngle>(key, KeyValuesType.TYPE_QANGLE, angle);

        public void SetMatrix3x4(string key, Matrix3x4 value) => SetValue<Matrix3x4>(key, KeyValuesType.TYPE_MATRIX3X4, value);
#endregion

        internal void SetValue<T>(string key, KeyValuesType type, T value)
        {
            List<object> arguments = new List<object>();

            switch (type)
            {
                case KeyValuesType.TYPE_EHANDLE:
                    {
                        if (value is CEntityHandle entityHandle)
                        {
                            arguments.Add(entityHandle.Raw);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_COLOR:
                    {
                        if (value is Color color)
                        {
                            arguments.Add(color.R);
                            arguments.Add(color.G);
                            arguments.Add(color.B);
                            arguments.Add(color.A);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_VECTOR:
                    {
                        if (value is Vector vector)
                        {
                            arguments.Add(vector.X);
                            arguments.Add(vector.Y);
                            arguments.Add(vector.Z);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_VECTOR2D:
                    {
                        // TODO: Change to 'Vector2D'
                        if (value is Vector2 vector)
                        {
                            arguments.Add(vector.X);
                            arguments.Add(vector.Y);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_VECTOR4D:
                    {
                        // TODO: Change to 'Vector4D'
                        if (value is Vector4 vector)
                        {
                            arguments.Add(vector.X);
                            arguments.Add(vector.Y);
                            arguments.Add(vector.Z);
                            arguments.Add(vector.W);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_QUATERNION:
                    {
                        // TODO: Change to 'Quaternion'
                        if (value is Vector4 vector)
                        {
                            arguments.Add(vector.X);
                            arguments.Add(vector.Y);
                            arguments.Add(vector.Z);
                            arguments.Add(vector.W);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_QANGLE:
                    {
                        if (value is QAngle angle)
                        {
                            arguments.Add(angle.X);
                            arguments.Add(angle.Y);
                            arguments.Add(angle.Z);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                case KeyValuesType.TYPE_MATRIX3X4:
                    {
                        if (value is Matrix3x4 matrix)
                        {
                            arguments.Add(matrix.M11);
                            arguments.Add(matrix.M12);
                            arguments.Add(matrix.M13);
                            arguments.Add(matrix.M14);
                            arguments.Add(matrix.M21);
                            arguments.Add(matrix.M22);
                            arguments.Add(matrix.M23);
                            arguments.Add(matrix.M24);
                            arguments.Add(matrix.M31);
                            arguments.Add(matrix.M32);
                            arguments.Add(matrix.M33);
                            arguments.Add(matrix.M34);
                        } else { BadTypeHandler(key, type, value); }
                    } break;

                default:
                    arguments.Add((object)value!);
                    break;
            }

            NativeAPI.EntityKeyValuesSetValue(key, (uint)type, arguments.ToArray());
        }

        internal T GetValue<T>(string key, KeyValuesType type, T defaultValue)
        {
            return NativeAPI.EntityKeyValuesGetValue<T>(key, (uint)type) ?? defaultValue;
        }

        internal void BadTypeHandler<T>(string key, KeyValuesType type, T value)
        {
            throw new ArgumentException($"Bad type for EntityKeyValues: got '{typeof(T)}' expected: '{type}'");
        }
    }
}
