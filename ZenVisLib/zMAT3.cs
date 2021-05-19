using System;
using System.Globalization;
using System.Linq;

namespace ZenVis.Lib
{
    public class zMAT3
    {
        public float[,] m;

        public zMAT3()
        {
            this.m = new float[3, 3];
        }

        public zMAT3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22) : this()
        {
            this.m[0, 0] = m00;
            this.m[0, 1] = m01;
            this.m[0, 2] = m02;
            this.m[1, 0] = m10;
            this.m[1, 1] = m11;
            this.m[1, 2] = m12;
            this.m[2, 0] = m20;
            this.m[2, 1] = m21;
            this.m[2, 2] = m22;
        }

        private static string FloatToHexString(float value)
        {
            return BitConverter.ToString(BitConverter.GetBytes(value)).Replace("-", "").ToLower();
        }

        public static zMAT3 FromString(string hexString)
        {
            return new zMAT3(zMAT3.HexStringToFloat(hexString, 0), zMAT3.HexStringToFloat(hexString, 8), zMAT3.HexStringToFloat(hexString, 16), zMAT3.HexStringToFloat(hexString, 24), zMAT3.HexStringToFloat(hexString, 32), zMAT3.HexStringToFloat(hexString, 40), zMAT3.HexStringToFloat(hexString, 48), zMAT3.HexStringToFloat(hexString, 56), zMAT3.HexStringToFloat(hexString, 64));
        }

        private static float HexStringToFloat(string hexString, int offset)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(uint.Parse(hexString.Substring(offset, 8), NumberStyles.AllowHexSpecifier)).Reverse<byte>().ToArray<byte>(), 0);
        }

        public new string ToString()
        {
            return string.Concat(new string[] { zMAT3.FloatToHexString(this.m[0, 0]), zMAT3.FloatToHexString(this.m[0, 1]), zMAT3.FloatToHexString(this.m[0, 2]), zMAT3.FloatToHexString(this.m[1, 0]), zMAT3.FloatToHexString(this.m[1, 1]), zMAT3.FloatToHexString(this.m[1, 2]), zMAT3.FloatToHexString(this.m[2, 0]), zMAT3.FloatToHexString(this.m[2, 1]), zMAT3.FloatToHexString(this.m[2, 2]) });
        }
    }
}