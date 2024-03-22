using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    /// <summary>
    /// Matrix3x4 for <see cref="CEntityKeyValues"/>
    /// </summary>
    public class Matrix3x4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;

        public Matrix3x4() { }

        public Matrix3x4(float M11, float M12, float M13, float M14, float M21, float M22, float M23, float M24, float M31, float M32, float M33, float M34)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13; this.M14 = M14; this.M21 = M21; this.M22 = M22; this.M23 = M23; this.M24 = M24; this.M31 = M31; this.M32 = M32; this.M33 = M33; this.M34 = M34;
        }
    }
}
