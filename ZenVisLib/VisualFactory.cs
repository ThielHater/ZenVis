using System.IO;
using System.Linq;
using System.Numerics;
using ZenVis.Shared;

namespace ZenVis.Lib
{
    public class VisualFactory
    {
        public VisualFactory()
        {
        }

        public static Visual GetVisual(string gothicDir, string gothicVersion, ref string[] lines)
        {
            Visual item;
            try
            {
                string str = VisualFactory.ReadType(ref lines);
                string item1 = VisualFactory.ReadFileName(ref lines);
                string extension = Path.GetExtension(item1);
                if (str.EndsWith("oCItem:zCVob"))
                {
                    if (item1 == "")
                    {
                        string str1 = VisualFactory.ReadItemInstance(ref lines, gothicVersion);
                        if (!Item.Instances[gothicVersion].ContainsKey(str1))
                        {
                            item = null;
                            return item;
                        }
                        else
                        {
                            item1 = Item.Instances[gothicVersion][str1];
                        }
                    }
                    item = new Item(item1, VisualFactory.ReadPosition(ref lines), VisualFactory.ReadRotation(ref lines), VisualFactory.ReadVisible(ref lines));
                }
                else if (!str.EndsWith("oCMOB:zCVob") || (item1 == ""))
                {
                    if (str.Contains("zCVobLevelCompo"))
                    {
                        item = new Level(item1, VisualFactory.ReadPosition(ref lines), VisualFactory.ReadRotation(ref lines), true);
                        return item;
                    }
                    else if (str.EndsWith("zCVob") && item1 != "" && extension != "")
                    {
                        if (extension == ".TGA")
                        {
                            item = new Decal(item1, VisualFactory.ReadPosition(ref lines), VisualFactory.ReadRotation(ref lines), VisualFactory.ReadDimensions(ref lines, gothicVersion), VisualFactory.ReadVisible(ref lines));
                            return item;
                        }
                        else if (extension != ".PFX")
                        {
                            item = new Vob(item1, VisualFactory.ReadPosition(ref lines), VisualFactory.ReadRotation(ref lines), VisualFactory.ReadVisible(ref lines));
                            return item;
                        }
                    }
                    item = null;
                }
                else
                {
                    item = new Mob(item1, VisualFactory.ReadPosition(ref lines), VisualFactory.ReadRotation(ref lines), VisualFactory.ReadVisible(ref lines));
                }
            }
            catch
            {
                item = null;
            }
            return item;
        }

        private static Vector3 ReadDimensions(ref string[] lines, string gothicVersion)
        {
            int idx = gothicVersion == "Gothic" ? 16 : 21;
            string[] array = (
                from x in Parser.RegVisualDimensions.Split(lines[idx])
                where !string.IsNullOrWhiteSpace(x)
                select x).ToArray<string>();
            return new Vector3(Helper.StringToFloat(array[0]) / 100f * 2f, Helper.StringToFloat(array[1]) / 100f * 2f, 0f);
        }

        private static string ReadFileName(ref string[] lines)
        {
            string[] array = (
                from x in Parser.RegVisualFileName.Split(lines[7])
                where !string.IsNullOrWhiteSpace(x)
                select x into y
                select y.ToUpper()).ToArray<string>();
            if (array.Length == 0)
            {
                return "";
            }
            return array[0];
        }

        private static string ReadItemInstance(ref string[] lines, string gothicVersion)
        {
            int idx = gothicVersion == "Gothic" ? 18 : 23;
            return (
                from x in Parser.RegVisualItemInstance.Split(lines[idx])
                where !string.IsNullOrWhiteSpace(x)
                select x).ToArray<string>()[0];
        }

        private static Vector3 ReadPosition(ref string[] lines)
        {
            string[] array = (
                from x in Parser.RegVisualPosition.Split(lines[5])
                where !string.IsNullOrWhiteSpace(x)
                select x).ToArray<string>();
            Vector3 vector3 = new Vector3()
            {
                X = Helper.StringToFloat(array[0]),
                Y = Helper.StringToFloat(array[1]),
                Z = Helper.StringToFloat(array[2])
            };
            vector3 /= 100f;
            return vector3;
        }

        private static zMAT3 ReadRotation(ref string[] lines)
        {
            return zMAT3.FromString((
                from x in Parser.RegVisualRotation.Split(lines[4])
                where !string.IsNullOrWhiteSpace(x)
                select x).ToArray<string>()[0]);
        }

        private static string ReadType(ref string[] lines)
        {
            return (
                from x in Parser.RegVisualType.Split(lines[0])
                where !string.IsNullOrWhiteSpace(x)
                select x).ToArray<string>()[0];
        }

        private static bool ReadVisible(ref string[] lines)
        {
            string[] array = (
                from x in Parser.RegVisualVisible.Split(lines[8])
                where !string.IsNullOrWhiteSpace(x)
                select x).ToArray<string>();
            if (array.Length == 0)
            {
                return true;
            }
            return array[0] == "1";
        }
    }
}