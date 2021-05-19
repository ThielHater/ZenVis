using System.Numerics;

namespace ZenVis.Lib
{
    public class Mob : Visual
    {
        public Mob(string fileName, Vector3 pos, zMAT3 rot3, bool visible) : base(fileName, pos, rot3, visible)
        {
        }
    }
}