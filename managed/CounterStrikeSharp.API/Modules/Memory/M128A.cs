using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace CounterStrikeSharp.API.Modules.Memory
{
    /// <summary>
    /// Represents a 128-bit value, typically used for XMM registers.
    /// This structure mirrors the layout of two 64-bit unsigned integers,
    /// similar to how XMM register data might be stored or pointed to in native code.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16, Pack = 16)]
    public struct M128A
    {
        /// <summary>
        /// The low 64 bits of the 128-bit value.
        /// </summary>
        public ulong Low;

        /// <summary>
        /// The high 64 bits of the 128-bit value.
        /// </summary>
        public ulong High;

        /// <summary>
        /// Initializes a new instance of the <see cref="M128A"/> struct with specified low and high 64-bit values.
        /// </summary>
        /// <param name="low">The low 64 bits.</param>
        /// <param name="high">The high 64 bits.</param>
        public M128A(ulong low, ulong high)
        {
            Low = low;
            High = high;
        }

        /// <summary>
        /// Reinterprets this M128A value as a <see cref="Vector128{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the vector. Must be an unmanaged type.</typeparam>
        /// <returns>A <see cref="Vector128{T}"/> representation of this M128A value.</returns>
        public unsafe Vector128<T> AsVector128<T>() where T : unmanaged
        {
            fixed (void* ptr = &this)
            {
                return Unsafe.Read<Vector128<T>>(ptr);
            }
        }

        /// <summary>
        /// Gets the float value at the specified zero-based index (0-3).
        /// </summary>
        /// <param name="index">The index of the float value (0, 1, 2, or 3).</param>
        /// <returns>The float value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not between 0 and 3.</exception>
        public unsafe float GetFloat(int index)
        {
            if ((uint)index >= 4)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 3.");
            
            fixed (void* ptr = &this)
            {
                return ((float*)ptr)[index];
            }
        }

        /// <summary>
        /// Gets the double value at the specified zero-based index (0-1).
        /// </summary>
        /// <param name="index">The index of the double value (0 or 1).</param>
        /// <returns>The double value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not 0 or 1.</exception>
        public unsafe double GetDouble(int index)
        {
            if ((uint)index >= 2)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 or 1.");

            fixed (void* ptr = &this)
            {
                return ((double*)ptr)[index];
            }
        }
        

        /// <summary>
        /// Gets the 32-bit signed integer value at the specified zero-based index (0-3).
        /// </summary>
        /// <param name="index">The index of the integer value (0, 1, 2, or 3).</param>
        /// <returns>The 32-bit signed integer value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not between 0 and 3.</exception>
        public unsafe int GetInt(int index)
        {
            if ((uint)index >= 4)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 3.");

            fixed (void* ptr = &this)
            {
                return ((int*)ptr)[index];
            }
        }

        /// <summary>
        /// Gets the 32-bit unsigned integer value at the specified zero-based index (0-3).
        /// </summary>
        /// <param name="index">The index of the unsigned integer value (0, 1, 2, or 3).</param>
        /// <returns>The 32-bit unsigned integer value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not between 0 and 3.</exception>
        public unsafe uint GetUInt(int index)
        {
            if ((uint)index >= 4)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 3.");

            fixed (void* ptr = &this)
            {
                return ((uint*)ptr)[index];
            }
        }

        /// <summary>
        /// Gets the 64-bit signed integer value at the specified zero-based index (0-1).
        /// Index 0 corresponds to the Low 64 bits, Index 1 to the High 64 bits.
        /// </summary>
        /// <param name="index">The index of the long value (0 or 1).</param>
        /// <returns>The 64-bit signed integer value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not 0 or 1.</exception>
        public long GetLong(int index)
        {
            return index switch
            {
                0 => (long)Low,
                1 => (long)High,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 or 1.")
            };
        }

        /// <summary>
        /// Gets the 64-bit unsigned integer value at the specified zero-based index (0-1).
        /// Index 0 corresponds to the Low 64 bits, Index 1 to the High 64 bits.
        /// </summary>
        /// <param name="index">The index of the ulong value (0 or 1).</param>
        /// <returns>The 64-bit unsigned integer value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not 0 or 1.</exception>
        public ulong GetULong(int index)
        {
            return index switch
            {
                0 => Low,
                1 => High,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 or 1.")
            };
        }
    }
}
