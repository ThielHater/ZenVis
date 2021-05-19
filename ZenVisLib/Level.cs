using System.Numerics;

namespace ZenVis.Lib
{
    public class Level : Visual
    {
        public Level(string fileName, Vector3 pos, zMAT3 rot3, bool visible) : base(fileName, pos, rot3, visible)
        {
            this.Scale = new Vector3(1f, 1f, 1f);
        }
    }
}
