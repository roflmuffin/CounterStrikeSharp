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
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;

namespace CounterStrikeSharp.API.Core
{
    public class InputArgument
    {
        protected object m_value;

        internal object Value => m_value;

        private InputArgument(object value)
        {
            m_value = value;
        }

        public override string ToString()
        {
            return m_value.ToString();
        }

        public static implicit operator InputArgument(bool value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(sbyte value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(byte value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(short value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(ushort value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(int value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(uint value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(long value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(ulong value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(float value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(double value)
        {
            return new InputArgument((float)value);
        }

        public static implicit operator InputArgument(Enum value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(string value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(Vector3 value)
        {
            return new InputArgument(value);
        }

        public static implicit operator InputArgument(Delegate value)
        {
            var functionReference = FunctionReference.Create(value);
            IntPtr cb = functionReference.GetFunctionPointer();
            return new InputArgument(cb.ToInt32());
        }

        public static implicit operator InputArgument(FunctionReference value)
        {
            IntPtr cb = value.GetFunctionPointer();
            return new InputArgument(cb.ToInt32());
        }

        [SecurityCritical]
        public static implicit operator InputArgument(IntPtr value)
        {
            return new InputArgument(value);
        }

        [SecurityCritical]
        public static unsafe implicit operator InputArgument(void* value)
        {
            return new InputArgument(new IntPtr(value));
        }
    }
}