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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Utils
{
    /// <summary>
    /// A <c>Vector</c> object represents a line with a direction and length.
    /// Each vector contains three co-ordinates:
    /// <list type="bullet">
    /// <item><term>X</term><description>+forward/-backward</description></item>
    /// <item><term>Y</term><description>+left/-right</description></item>
    /// <item><term>Z</term><description>+up/-down</description></item>
    /// </list>
    /// </summary>
    public class Vector : NativeObject
    {
        public Vector(IntPtr pointer) : base(pointer)
        {
        }


        public Vector(float? x = null, float? y = null, float? z = null) : this(NativeAPI.VectorNew())
        {
            this.X = x ?? 0;
            this.Y = y ?? 0;
            this.Z = z ?? 0;
        }

        public unsafe ref float X => ref Unsafe.Add(ref *(float*)Handle, 0);
        public unsafe ref float Y => ref Unsafe.Add(ref *(float*)Handle, 1);
        public unsafe ref float Z => ref Unsafe.Add(ref *(float*)Handle, 2);
        
        /// <summary>
        /// Returns a copy of the vector with values replaced.
        /// </summary>
        /// <param name="x">X value to replace with</param>
        /// <param name="y">Y value to replace with</param>
        /// <param name="z">Z value to replace with</param>
        /// <returns>Copy of vector</returns>
        public Vector With(float? x = null, float? y = null, float? z = null)
        {
            var retVal = new Vector();
            retVal.X = x ?? this.X;
            retVal.Y = y ?? this.Y;
            retVal.Z = z ?? this.Z;

            return retVal;
        }


        /// <summary>
        /// Adds values of argument vector to the original vector. Does not create a new vector object, skipping object construction.
        /// </summary>
        /// <param name="vector"></param>
        public void Add(Vector vector)
        {
            this.X += vector.X;
            this.Y += vector.Y;
            this.Z = this.Z = vector.Z;
        }

        
        /// <summary>
        /// Returns an angle that represents the normal of the vector.
        /// </summary>
        /// <returns></returns>
        public Angle Angle()
        {
            var angle = new Angle();
            NativeAPI.VectorAngles(Handle, IntPtr.Zero, angle.Handle);
            return angle;
        }

        /// <summary>
        /// Returns the angle of the vector, but allows the use of a different 'up' direction.
        /// </summary>
        /// <param name="up">Direction for up</param>
        /// <returns></returns>
        public Angle Angle(Vector up)
        {
            var angle = new Angle();
            NativeAPI.VectorAngles(Handle, up.Handle, angle.Handle);
            return angle;
        }

        /*

        /// <summary>
        /// Calculates the cross product of the vector and the passed vector.
        /// </summary>
        /// <param name="otherVector"></param>
        /// <returns></returns>
        public Vector Cross(Vector otherVector)
        {
            return NativePINVOKE.Vector_Cross(ptr, otherVector.Handle()).ToObject<Vector>();
        }

        /// <summary>
        /// Returns euclidean distance between this vector and another vector. Relatively expensive.
        /// </summary>
        /// <param name="otherVector"></param>
        /// <returns></returns>
        public float Distance(Vector otherVector)
        {
            return NativePINVOKE.Vector_DistTo(ptr, otherVector.Handle());
        }

        /// <summary>
        /// Returns squared distance of 2 vectors. Faster than <see cref="Distance"/>
        /// </summary>
        /// <param name="otherVector"></param>
        /// <returns></returns>
        public float DistToSqr(Vector otherVector)
        {
            return NativePINVOKE.Vector_DistToSqr(ptr, otherVector.Handle());
        }

        /// <summary>
        /// Divides all values of the Vector by a divisor. Does not create a new vector object, skipping object construction.
        /// </summary>
        /// <param name="divisor"></param>
        public void Div(float divisor)
        {
            var returnVal = new Vector();
            NativePINVOKE.VectorDivide__SWIG_0(ptr, divisor, returnVal.Handle());
            Set(returnVal);
        }

        /// <summary>
        /// Returns the dot product of this vector and the supplied vector.
        /// </summary>
        /// <param name="otherVector"></param>
        /// <returns>Dot product</returns>
        public float Dot(Vector otherVector)
        {
            return NativePINVOKE.Vector_Dot(ptr, otherVector.Handle());
        }

        /// <summary>
        /// Returns a normalized version of the vector. Note this does not affect the vector it is called on.
        /// </summary>
        /// <returns>Normalized vector</returns>
        public Vector GetNormalized()
        {
            return NativePINVOKE.Vector_Normalized(ptr).ToObject<Vector>();
        }

        /// <summary>
        /// Returns if the Vector is equal to another vector with the given tolerance.
        /// </summary>
        /// <param name="compare">Angle to compare with</param>
        /// <param name="tolerance">Allowed tolerance when comparing equality</param>
        /// <returns></returns>
        public bool IsEqualTol(Vector compare, float tolerance)
        {
            return Math.Abs(X - compare.X) < tolerance &&
            Math.Abs(Y - compare.Y) < tolerance &&
            Math.Abs(Z - compare.Z) < tolerance;
        }
        */
        /// <summary>
        /// Returns whether all fields on the Vector are 0.
        /// </summary>
        /// <returns></returns>
        public bool IsZero() => NativeAPI.VectorIsZero(Handle);

        /// <summary>
        /// Returns the Euclidean length of the vector: √x² + y² + z² 
        /// </summary>
        /// <returns>Euclidean length of vector</returns>
        public float Length() => NativeAPI.VectorLength(Handle);

        /// <summary>
        /// Returns length of Vector excluding Z axis.
        /// </summary>
        /// <returns>2D Length</returns>
        public float Length2D() => NativeAPI.VectorLength2d(Handle);

        /// <summary>
        /// Returns the squared length of the vector, x² + y² + z². Faster than <see cref="Length"/>
        /// </summary>
        /// <returns></returns>
        public float LengthSqr() => NativeAPI.VectorLengthSqr(Handle);

        /// <summary>
        /// Returns the squared length of the vectors x and y value, x² + y². Faster than <see cref="Length2D"/>
        /// </summary>
        /// <returns></returns>
        public float Length2DSqr() => NativeAPI.VectorLength2dSqr(Handle);

        /*

        /// <summary>
        /// Multiplies all values in the vector by a multiplier. Does not create a new vector object, skipping object construction.
        /// </summary>
        /// <param name="multiplier">value to multiply by</param>
        public void Mul(float multiplier)
        {
            var returnVal = new Vector();
            NativePINVOKE.VectorMultiply__SWIG_0(ptr, multiplier, returnVal.Handle());
            Set(returnVal);
        }

        /// <summary>
        /// Normalizes the vector. This will modify the Vector you call it on; to receive a
        /// normalized copy without modification, use <see cref="GetNormalized"/>
        /// </summary>
        public void Normalize()
        {
            NativePINVOKE.Vector_NormalizeInPlace(ptr);
        }

        /// <summary>
        /// Rotates the vector by the given angle
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(Angle angle)
        {
            var returnVal = new Vector();
            NativePINVOKE.VectorRotate__SWIG_1(ptr, angle.Handle(), returnVal.Handle());
            Set(returnVal);
        }        

        /// <summary>
        /// Copies X, Y and Z into this Vector
        /// </summary>
        /// <param name="vector">angle to copy from</param>
        public void Set(Vector vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        /// <summary>
        /// Sets the <paramref name="x"/>, <paramref name="y"/> and <paramref name="z"/> of the Vector
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public void Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Subtracts the values of the supplied vector from the original vector. Does not create a new vector object, skipping object construction.
        /// </summary>
        /// <param name="vector"></param>
        public void Sub(Vector vector)
        {
            this.X -= vector.X;
            this.Y -= vector.Y;
            this.Z -= vector.Z;
        }

        /// <summary>
        /// Returns whether the given vector is within a box created by the supplied vectors.
        /// </summary>
        /// <param name="boxStart">Box Start</param>
        /// <param name="boxEnd">Box End</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public bool WithinAABox(Vector boxStart, Vector boxEnd)
        {
            return NativePINVOKE.Vector_WithinAABox(ptr, boxStart.Handle(), boxEnd.Handle());
        }

        /// <summary>
        /// Zeroes out the X, Y and Z values of the vector
        /// </summary>
        public void Zero()
        {
            NativePINVOKE.Vector_Zero(ptr);
        }
        */
        #region Operators

        public float this[int i]
        {
            get {
                switch (i)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                        case 2: 
                    return this.Z;
                }

                return 0;
            }
            set {
                 switch (i)
                {
                    case 0:
                        this.X = value;
                        break;
                    case 1:
                        this.Y = value;
                        break;
                    case 2: 
                        this.Z = value;
                        break;
                }
            }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y, -a.Z);
        }

        public static Vector operator *(Vector a, float b)
        {
            return new Vector(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector operator /(Vector a, float b)
        {
            return new Vector(a.X / b, a.Y / b, a.Z / b);
        }

        #endregion
        /*

        public override bool Equals(object obj)
        {
            Vector v = obj as Vector;
            if (v == null)
                return false;

            return this.IsEqualTol(v, 0.01f);
        }

        */

        public override string ToString()
        {
            return $"{X:n2} {Y:n2} {Z:n2}";
        }

        /*       

        */
    }
}