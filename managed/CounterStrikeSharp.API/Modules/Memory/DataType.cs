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

using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Memory
{
    public enum DataType
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
    }

    public static class DataTypeExtensions
    {
        private static Dictionary<Type, DataType> types = new Dictionary<Type, DataType>()
        {
            { typeof(float), DataType.DATA_TYPE_FLOAT },
            { typeof(IntPtr), DataType.DATA_TYPE_POINTER },
            { typeof(int), DataType.DATA_TYPE_INT },
            { typeof(uint), DataType.DATA_TYPE_UINT },
            { typeof(bool), DataType.DATA_TYPE_BOOL },
            { typeof(string), DataType.DATA_TYPE_STRING },
            { typeof(long), DataType.DATA_TYPE_LONG },
            { typeof(ulong), DataType.DATA_TYPE_ULONG },
            { typeof(short), DataType.DATA_TYPE_SHORT },
            { typeof(sbyte), DataType.DATA_TYPE_UCHAR },
            { typeof(byte), DataType.DATA_TYPE_CHAR },
        };

        public static DataType? ToDataType(this Type type)
        {
            if (types.ContainsKey(type)) return types[type];

            if (typeof(NativeObject).IsAssignableFrom(type))
            {
                return DataType.DATA_TYPE_POINTER;
            }

            if (type.IsEnum && types.ContainsKey(Enum.GetUnderlyingType(type)))
            {
                return types[Enum.GetUnderlyingType(type)];
            }
            
            Core.Application.Instance.Logger.LogWarning("Error retrieving data type for type {Type}", type.FullName);

            return null;
        }
        
        public static DataType ToValidDataType(this Type type)
        {
            if (types.ContainsKey(type)) return types[type];

            if (typeof(NativeObject).IsAssignableFrom(type))
            {
                return DataType.DATA_TYPE_POINTER;
            }

            if (type.IsEnum && types.ContainsKey(Enum.GetUnderlyingType(type)))
            {
                return types[Enum.GetUnderlyingType(type)];
            }
            
            throw new NotSupportedException("Data type not supported:" + type.FullName);
        }
    }
}