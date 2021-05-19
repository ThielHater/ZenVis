using System.Numerics;

namespace ZenVis.Lib
{
    public class Decal : Visual
    {
        public Vector3 Dimensions;

        public Decal(string fileName, Vector3 pos, zMAT3 rot3, Vector3 dim, bool visible) : base(fileName, pos, rot3, visible)
        {
            this.Dimensions = dim;
        }
    }
}