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
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils
{
    /// <summary>
    /// A <c>Angle</c> object represents 3D Euler angle, offset from the cardinal Z axis.
    /// Each angle contains three rotation values.
    /// <list type="bullet">
    /// <item><term>X</term><description>pitch +down/-up</description></item>
    /// <item><term>Y</term><description>yaw +left/-right</description></item>
    /// <item><term>Z</term><description>roll +right/-left</description></item>
    /// </list>
    /// </summary>
    public class Angle : NativeObject
    {
        public Angle(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Creates new angle with the supplied Pitch, Yaw and Roll values.
        /// </summary>
        /// <param name="x">Pitch</param>
        /// <param name="y">Yaw</param>
        /// <param name="z">Roll</param>
        public Angle(float? x = null, float? y = null, float? z = null) : this(NativeAPI.AngleNew())
        {
            this.X = x ?? 0;
            this.Y = y ?? 0;
            this.Z = z ?? 0;
        }
        
        

        #region Accessors

        /// <summary>
        /// Pitch of angle
        /// </summary>
        public float Pitch
        {
            get => X;
            set => X = value;
        }

        /// <summary>
        /// Yaw of angle
        /// </summary>
        public float Yaw
        {
            get => Y;
            set => Y = value;
        }

        /// <summary>
        /// Roll of angle
        /// </summary>
        public float Roll
        {
            get => Z;
            set => Z = value;
        }

        /// <summary>
        /// Pitch of angle
        /// </summary>
        public float P
        {
            get => X;
            set => X = value;
        }
        
        /// <summary>
        /// Roll of angle
        /// </summary>
        public float R
        {
            get => Z;
            set => Z = value;
        }

        #endregion

        /// <summary>
        /// Pitch of angle
        /// </summary>
        public float X
        {
            set => NativeAPI.VectorSetX(Handle, value);
            get => NativeAPI.VectorGetX(Handle);
        }

        /// <summary>
        /// Yaw of angle
        /// </summary>
        public float Y
        {
            set => NativeAPI.VectorSetY(Handle, value);
            get => NativeAPI.VectorGetY(Handle);
        }

        /// <summary>
        /// Roll of angle
        /// </summary>
        public float Z
        {
            set => NativeAPI.VectorSetZ(Handle, value);
            get => NativeAPI.VectorGetZ(Handle);
        }

        /*
        /// <summary>
        /// Returns a copy of the angle with values replaced.
        /// </summary>
        /// <param name="x">Pitch value to replace with</param>
        /// <param name="y">Yaw value to replace with</param>
        /// <param name="z">Roll value to replace with</param>
        /// <returns>Copy of vector</returns>
        public Angle With(float? x = null, float? y = null, float? z = null)
        {
            var retVal = new Angle();
            retVal.X = x ?? this.X;
            retVal.Y = y ?? this.Y;
            retVal.Z = z ?? this.Z;

            return retVal;
        }

        /// <summary>
        /// Adds the values of argument angle to the original angle. Does not create a new angle object, skipping object construction
        /// </summary>
        /// <param name="angle">Angle values to add</param>
        public void Add(Angle angle)
        {
            X += angle.X;
            Y += angle.Y;
            Z += angle.Z;

        }

        /// <summary>
        /// Divides all values of the angle by a divisor. Does not create a new angle object, skipping object construction.
        /// </summary>
        /// <param name="divisor"></param>
        public void Div(float divisor)
        {
            X /= divisor;
            Y /= divisor;
            Z /= divisor;
        }

        /// <summary>
        /// Returns a normal vector facing in the direction that the angle is pointing.
        /// </summary>
        /// <returns>Normal vector</returns>
        public Vector Forward()
        {
            var vec = new Vector();
            NativePINVOKE.AngleVectors__SWIG_0(ptr, vec.Handle());
            return vec;
        }

        /// <summary>
        /// Returns whether the pitch, yaw and roll are all 0.
        /// </summary>
        /// <returns></returns>
        public bool IsZero()
        {
            var tolerance = 0.01f;
            return (X > -tolerance && X < tolerance &&
				Y > -tolerance && Y < tolerance &&
				Z > -tolerance && Z < tolerance);
        }

        /// <summary>
        /// Multiplies all values in the angle by a multiplier. Does not create a new angle object, skipping object construction.
        /// </summary>
        /// <param name="multiplier">value to multiply by</param>
        public void Mul(float multiplier)
        {
            X *= multiplier;
            Y *= multiplier;
            Z *= multiplier;
        }

        /// <summary>
        /// Normalizes the angles by applying a modulo of 360 to pitch, yaw and roll.
        /// </summary>
        public void Normalize()
        {
            X = NativePINVOKE.AngleNormalize(X);
            Y = NativePINVOKE.AngleNormalize(Y);
            Z = NativePINVOKE.AngleNormalize(Z);
        }

        /// <summary>
        /// Returns if the angle is equal to another angle with the given tolerance.
        /// </summary>
        /// <param name="compare">Angle to compare with</param>
        /// <param name="tolerance">Allowed tolerance when comparing equality</param>
        /// <returns></returns>
        public bool IsEqualTol(Angle compare, float tolerance)
        {
            return Math.Abs(X - compare.X) < tolerance &&
                   Math.Abs(Y - compare.Y) < tolerance &&
                   Math.Abs(Z - compare.Z) < tolerance;
        }

        /// <summary>
        /// Returns a normal vector facing in the direction that points right relative to the angle's direction.
        /// </summary>
        /// <returns>Right vector</returns>
        public Vector Right()
        {
            var right = new Vector();
            NativePINVOKE.AngleVectors__SWIG_1(ptr, IntPtr.Zero.ToHandle(), right.Handle(), IntPtr.Zero.ToHandle());
            return right;
        }

        /// <summary>
        /// Rotates the angle around a specified axis by degrees specified.
        /// </summary>
        /// <param name="axis">axis to rotate around</param>
        /// <param name="rotation">degrees to rotate by</param>
        public void RotateAroundAxis(Vector axis, float rotation)
        {
            var returnVal = new Angle();
            NativePINVOKE.RotationDeltaAxisAngleRef(ptr, returnVal.Handle(), axis.Handle(), rotation);
            Set(returnVal);
        }

        /// <summary>
        /// Copies pitch, yaw and roll into this angle
        /// </summary>
        /// <param name="angle">angle to copy from</param>
        public void Set(Angle angle)
        {
            this.X = angle.X;
            this.Y = angle.Y;
            this.Z = angle.Z;
        }

        /// <summary>
        /// Sets the pitch, yaw and roll.
        /// </summary>
        /// <param name="x">pitch/x</param>
        /// <param name="y">yaw/y</param>
        /// <param name="z">roll/z</param>
        public void Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Subtracts the pitch,yaw and roll values from the supplied angle.
        /// </summary>
        /// <param name="angle">Angle to subtract</param>
        public void Sub(Angle angle)
        {
            this.X -= angle.X;
            this.Y -= angle.Y;
            this.Z -= angle.Z;
        }

        /// <summary>
        /// Returns a normal vector facing in the direction that points up relative to the angle's direction.
        /// </summary>
        /// <returns>Up vector</returns>
        public Vector Up()
        {
            var up = new Vector();
            NativePINVOKE.AngleVectors__SWIG_1(ptr, IntPtr.Zero.ToHandle(), IntPtr.Zero.ToHandle(), up.Handle());
            return up;
        }

        /// <summary>
        /// Sets the pitch, yaw and roll to 0.
        /// </summary>
        public void Zero()
        {
            X = Y = Z = 0.0f;
        }

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

        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Angle operator -(Angle a, Angle b)
        {
            return new Angle(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Angle operator -(Angle a)
        {
            return new Angle(-a.X, -a.Y, -a.Z);
        }

        public static Angle operator *(Angle a, float b)
        {
            return new Angle(a.X * b, a.Y * b, a.Z * b);
        }
        public static Angle operator /(Angle a, float b)
        {
            return new Angle(a.X / b, a.Y / b, a.Z / b);
        }

        public override bool Equals(object obj)
        {
            Angle v = obj as Angle;
            if (v == null)
                return false;

            return this.IsEqualTol(v, 0.01f);
        }

        public override string ToString()
        {
            return $"{X:n2} {Y:n2} {Z:n2}";
        }

        #endregion

        protected override void OnDispose()
        {
        }*/
    }
}