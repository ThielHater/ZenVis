using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ZenVis.Shared;
using ZenVisLib.Properties;

namespace ZenVis.Lib
{
    public class Item : Visual
    {
        public static Dictionary<string, Dictionary<string, string>> Instances;

        static Item()
        {
            Item.Instances = Helper.DeserializeFromByteArray<Dictionary<string, Dictionary<string, string>>>(Resources.ItemInstances);
        }

        public Item(string fileName, Vector3 pos, zMAT3 rot3, bool visible) : base(fileName, pos, rot3, visible)
        {
            // Items are always visible, even if showVisual=bool:0
            Visible = true;
        }

        public static Dictionary<string, string> GetInstancesFromScripts(string gothicDir)
        {
            string[] files = Directory.GetFiles(string.Concat(gothicDir, "_WORK\\DATA\\SCRIPTS\\CONTENT\\ITEMS"), "*.D");
            Dictionary<string, string> strs = new Dictionary<string, string>();
            Dictionary<string, string> strs1 = new Dictionary<string, string>();
            string[] strArrays = files;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string[] strArrays1 = File.ReadAllLines(strArrays[i]);
                string str = "";
                string array = "";
                string str1 = "";
                for (int j = 0; j < (int)strArrays1.Length; j++)
                {
                    if (Parser.RegItemInstance.IsMatch(strArrays1[j]))
                    {
                        string[] array1 = (
                            from x in Parser.RegItemInstance.Split(strArrays1[j])
                            where !string.IsNullOrWhiteSpace(x)
                            select x into y
                            select y.ToUpper()).ToArray<string>();
                        if (array1[1] != "C_ITEM")
                        {
                            strs.Add(array1[0], strs1[array1[1]]);
                        }
                        else
                        {
                            str = array1[0];
                        }
                        str1 = "INSTANCE";
                    }
                    else if (Parser.RegItemPrototype.IsMatch(strArrays1[j]))
                    {
                        array = (
                            from x in Parser.RegItemPrototype.Split(strArrays1[j])
                            where !string.IsNullOrWhiteSpace(x)
                            select x into y
                            select y.ToUpper()).ToArray<string>()[0];
                        str1 = "PROTOTYPE";
                    }
                    else if (Parser.RegItemVisual.IsMatch(strArrays1[j]))
                    {
                        string[] array2 = (
                            from x in Parser.RegItemVisual.Split(strArrays1[j])
                            where !string.IsNullOrWhiteSpace(x)
                            select x into y
                            select y.ToUpper()).ToArray<string>();
                        if (str1 == "INSTANCE")
                        {
                            if (!strs.ContainsKey(str))
                            {
                                strs.Add(str, array2[0]);
                            }
                            else
                            {
                                strs[str] = array2[0];
                            }
                        }
                        else if (str1 == "PROTOTYPE")
                        {
                            strs1.Add(array, array2[0]);
                        }
                    }
                }
            }
            return strs;
        }
    }
}