using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;
using ZenVis.Shared;
using ZenVisLib.Properties;

namespace ZenVis.Lib
{
    public class ZenVisImp : ZenVis
    {
        public string ZSlangExe;

        public ZenVisImp(string appDir, string gothicDir, string gothicVersion, string blenderExe, string outputDir, string zSlangExe) : base(appDir, gothicDir, gothicVersion, blenderExe, outputDir)
        {
            this.ZSlangExe = zSlangExe;
            Directory.CreateDirectory(string.Concat(this.AppDir, "Import"));
        }

        public static bool CheckVariableValue(string typeStr, string valueStr)
        {
            bool flag;
            try
            {
                Type variableType = ZenVisImp.GetVariableType(typeStr);
                if (variableType == typeof(int))
                {
                    int.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (variableType == typeof(int[]))
                {
                    string[] strArrays = valueStr.Split(new char[] { ' ' });
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        int.Parse(strArrays[i], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                }
                else if (variableType == typeof(float))
                {
                    float.Parse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                else if (variableType == typeof(float[]) || variableType == typeof(Vector3))
                {
                    string[] strArrays1 = valueStr.Split(new char[] { ' ' });
                    if (!(variableType == typeof(Vector3)) || (int)strArrays1.Length == 3)
                    {
                        for (int j = 0; j < (int)strArrays1.Length; j++)
                        {
                            float.Parse(strArrays1[j], NumberStyles.Float, CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        flag = false;
                        return flag;
                    }
                }
                else if (variableType == typeof(zMAT3) && (!Parser.RegHexadecimal.IsMatch(valueStr) || valueStr.Length != 72))
                {
                    flag = false;
                    return flag;
                }
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public void GetEngineTypesFromZen(string json)
        {
            Dictionary<string, Dictionary<string, string>> strs = new Dictionary<string, Dictionary<string, string>>();
            string[] files = Directory.GetFiles(string.Concat(Path.GetDirectoryName(this.ZSlangExe), "\\worlds\\vobclasses"), "*.ZEN");
            for (int i = 0; i < (int)files.Length; i++)
            {
                string[] strArrays = File.ReadAllLines(files[i]);
                for (int j = 0; j < (int)strArrays.Length; j++)
                {
                    if (Parser.RegVisualType.IsMatch(strArrays[j]))
                    {
                        string[] array = (
                            from a in Parser.RegVisualType.Split(strArrays[j])
                            where !string.IsNullOrWhiteSpace(a)
                            select a).ToArray<string>();
                        string str = (array[0].IndexOf(':') > -1 ? array[0].Substring(0, array[0].IndexOf(':')) : array[0]);
                        if (!(str == "oCWorld:zCWorld") && !(str == "zCWayNet") && !strs.ContainsKey(str))
                        {
                            string str1 = strArrays[j].Substring(0, strArrays[j].Length - strArrays[j].TrimStart(Array.Empty<char>()).Length);
                            int num = j;
                            while (num < (int)strArrays.Length)
                            {
                                if (strArrays[num] != string.Concat(str1, "[]"))
                                {
                                    num++;
                                }
                                else
                                {
                                    string[] strArrays1 = strArrays.SubArray<string>(j, num - j + 1);
                                    strs.Add(str, this.GetProperties(strArrays1));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            File.WriteAllText(json, Helper.SerializeToString(strs));
        }

        public Dictionary<string, string> GetProperties(string[] lines)
        {
            Dictionary<string, string> strs = new Dictionary<string, string>();
            for (int i = 0; i < (int)lines.Length; i++)
            {
                if (lines[i].IndexOf(':') > -1)
                {
                    lines[i] = lines[i].Substring(0, lines[i].IndexOf(':') + 1);
                }
                if (Parser.RegProperty.IsMatch(lines[i]))
                {
                    string[] array = (
                        from a in Parser.RegProperty.Split(lines[i])
                        where !string.IsNullOrWhiteSpace(a)
                        select a).ToArray<string>();
                    strs.Add(array[0], array[1]);
                }
            }
            return strs;
        }

        public static Type GetVariableType(string type)
        {
            if (type == "string")
            {
                return typeof(string);
            }
            if (type == "int")
            {
                return typeof(int);
            }
            if (type == "bool")
            {
                return typeof(int);
            }
            if (type == "enum")
            {
                return typeof(int);
            }
            if (type == "color")
            {
                return typeof(int[]);
            }
            if (type == "float")
            {
                return typeof(float);
            }
            if (type == "rawFloat")
            {
                return typeof(float[]);
            }
            if (type == "raw")
            {
                return typeof(zMAT3);
            }
            if (type == "vec3")
            {
                return typeof(Vector3);
            }
            throw new ArgumentException();
        }

        public void Import(string file, bool quiet)
        {
            string upper = Path.GetExtension(file).ToUpper();
            string str = string.Concat(this.AppDir, "Import\\", Path.GetFileNameWithoutExtension(file), ".blend");
            string str1 = string.Concat(this.AppDir, "Import\\", Path.GetFileNameWithoutExtension(file), "_Objects.json");
            string str2 = string.Concat(this.AppDir, "Import\\", Path.GetFileNameWithoutExtension(file), "_Settings.json");
            string str3 = string.Concat(this.AppDir, "Import\\", Path.GetFileNameWithoutExtension(file), ".zsl");
            string str4 = string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(file), ".3ds");
            string str5 = string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(file), ".zen");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("import bpy");
            stringBuilder.AppendLine("import math");
            stringBuilder.AppendLine("import mathutils");
            stringBuilder.AppendLine("import sys");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("argv = sys.argv");
            stringBuilder.AppendLine("argv = argv[argv.index(\"--\") + 1:]");
            stringBuilder.AppendLine("");
            if (upper == ".BLEND")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.wm.open_mainfile(filepath='", file.Replace("\\", "/"), "')"));
            }
            else if (upper == ".DAE")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.wm.collada_import(filepath='", file.Replace("\\", "/"), "')"));
            }
            else if (upper == ".FBX")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.import_scene.fbx(filepath='", file.Replace("\\", "/"), "', use_image_search=False)"));
            }
            else if (upper == ".OBJ")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.import_scene.obj(filepath='", file.Replace("\\", "/"), "', use_image_search=False)"));
            }
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("for obj in bpy.data.objects:");
            stringBuilder.AppendLine("    obj.name = obj.name.upper()");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("    if not obj.name.upper().startswith('LEVEL'):");
            stringBuilder.AppendLine("        if ((obj.scale[0] > 0) and (obj.scale[1] > 0) and (obj.scale[2] > 0)):");
            stringBuilder.AppendLine("            obj.scale = mathutils.Vector((-obj.scale[0], -obj.scale[1], obj.scale[2]))");
            stringBuilder.AppendLine("            obj.rotation_euler[0] = -obj.rotation_euler[0]");
            stringBuilder.AppendLine("            obj.rotation_euler[1] = -obj.rotation_euler[1]");
            stringBuilder.AppendLine("            obj.rotation_euler[2] += 3.14159265359");
            stringBuilder.AppendLine("        else:");
            stringBuilder.AppendLine("            print('Unknown scale: ' + str(obj.scale))");
            stringBuilder.AppendLine("        obj.rotation_euler[0] -= 1.5707963268");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("bpy.ops.wm.save_as_mainfile(filepath=argv[1], check_existing=False)");
            File.WriteAllText(string.Concat(this.AppDir, "Blender\\import_prepare_and_save_as_blend.py"), stringBuilder.ToString());
            Process process = new Process();
            process.StartInfo.FileName = this.BlenderExe;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.BlenderExe);
            process.StartInfo.Arguments = string.Concat(new string[] { Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\Empty.blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\import_prepare_and_save_as_blend.py")), " -- ", Helper.EscapeArgument(file), " ", Helper.EscapeArgument(str) });
            process.Start();
            process.WaitForExit();
            if (File.Exists(string.Concat(str, "1")))
            {
                File.Delete(string.Concat(str, "1"));
            }
            process.StartInfo.Arguments = string.Concat(new string[] { Helper.EscapeArgument(str), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\export_objects_as_json.py")), " -- ", Helper.EscapeArgument(str1) });
            process.Start();
            process.WaitForExit();

            List<Visual> list = (
                from x in Helper.DeserializeFromString<List<Visual>>(File.ReadAllText(str1))
                orderby x.FileName
                select x).Where(x => !x.FileName.StartsWith("!")).ToList<Visual>();

            if (list.Any(x => x.FileName.StartsWith("LEVEL")))
            {
                process.StartInfo.Arguments = string.Concat(new string[] { Helper.EscapeArgument(str), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\export_level_as_3ds.py")), " -- ", Helper.EscapeArgument(str4) });
                process.Start();
                while (!process.HasExited && Process.GetProcessesByName("wxImpExpUI").Length == 0)
                {
                    Thread.Sleep(100);
                }
                if (!process.HasExited)
                {
                    Window window = TestStack.White.Application.Attach("wxImpExpUI").GetWindow("Kerrax 3D Studio Mesh Exporter");
                    Helper.ResetWindow(window.Title);
                    window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-206")).Click();
                    window.Get<TestStack.White.UIItems.RadioButton>(SearchCriteria.ByAutomationId("-209")).Click();
                    window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-214")).Click();
                    Window window1 = window.ModalWindow("Space Transformation");
                    window1.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-233")).Text = "100";
                    window1.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-234")).Click();
                    window.Get<TestStack.White.UIItems.CheckBox>(SearchCriteria.ByAutomationId("-220")).Checked = false;
                    window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-224")).Click();
                    process.WaitForExit();
                }
            }

            Dictionary<string, Dictionary<string, string>> strs = Helper.DeserializeFromByteArray<Dictionary<string, Dictionary<string, string>>>(Resources.TypesProperties);
            Dictionary<string, ImportSetting> strs1 = new Dictionary<string, ImportSetting>();
            if (!quiet)
            {
                if (File.Exists(str2))
                    strs1 = Helper.DeserializeFromString<Dictionary<string, ImportSetting>>(File.ReadAllText(str2));
                (new ImportForm(strs1, strs, list)).ShowDialog();
                File.WriteAllText(str2, Helper.SerializeToString(strs1));
            }
            string fileName = "";
            string str6 = "";
            int num = 0;
            stringBuilder.Clear();
            stringBuilder.AppendLine("#include<stdlib.zsl>");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("func void main() {");

            if (list.Any(x => x.FileName.StartsWith("LEVEL")))
            {
                stringBuilder.AppendLine(string.Concat("var object ", Path.GetFileNameWithoutExtension(file), " = WLD_NewVobOfClass(\"zCVobLevelCompo\");"));
                stringBuilder.AppendLine(string.Concat(Path.GetFileNameWithoutExtension(file), ".visual = \"", Path.GetFileNameWithoutExtension(file), ".3DS\";"));
                stringBuilder.AppendLine("");
            }

            foreach (Visual visual in list)
            {
                if (visual.FileName.StartsWith("LEVEL"))
                {
                    continue;
                }
                if (fileName != visual.FileName)
                {
                    fileName = visual.FileName;
                    num = 0;
                }
                str6 = string.Concat(visual.FileName, (num == 0 ? "" : string.Concat("_", string.Format("{0:000}", num))));
                string[] strArrays = new string[] { "var object ", str6, " = WLD_NewVob(", null, null };
                strArrays[3] = (strs1.ContainsKey(visual.FileName) ? string.Concat("\"", strs1[visual.FileName].Type, "\"") : "");
                strArrays[4] = ");";
                stringBuilder.AppendLine(string.Concat(strArrays));
                string[] fileName1 = new string[] { str6, ".visual = \"", visual.FileName, ".", null, null };

                if (!string.IsNullOrEmpty(visual.FileExtension))
                {
                    fileName1[4] = visual.FileExtension;
                }
                else
                {
                    if (strs1.ContainsKey(visual.FileName) && strs1[visual.FileName].Type.Contains("Mob"))
                    {
                        fileName1[4] = "ASC";
                    }
                    else
                    {
                        fileName1[4] = "3DS";
                    }
                }

                fileName1[5] = "\";";
                stringBuilder.AppendLine(string.Concat(fileName1));
                stringBuilder.AppendLine(str6 + ".vobName = \"" + visual.FileName + "." + visual.Index + "\";");
                stringBuilder.AppendLine(string.Concat(new string[] { str6, ".trafoOSToWSPos = { ", Helper.FloatToString(visual.Position.X), ", ", Helper.FloatToString(visual.Position.Y), ", ", Helper.FloatToString(visual.Position.Z), " };" }));
                stringBuilder.AppendLine(string.Concat(str6, ".trafoOSToWSRot = \"", visual.Rotation.ToString(), "\";"));
                if (strs1.ContainsKey(visual.FileName))
                {
                    foreach (KeyValuePair<string, string> property in strs1[visual.FileName].Properties)
                    {
                        Type type = ZenVisImp.GetVariableType(strs[strs1[visual.FileName].Type][property.Key]);
                        if (type == typeof(string) || type == typeof(zMAT3))
                        {
                            stringBuilder.AppendLine(string.Concat(new string[] { str6, ".", property.Key, " = \"", property.Value.Replace("%FileName%", visual.FileName).Replace("%FileExtension%", visual.FileExtension).Replace("%Index%", visual.Index.ToString()), "\";" }));
                        }
                        else if (type == typeof(int) || type == typeof(float))
                        {
                            stringBuilder.AppendLine(string.Concat(new string[] { str6, ".", property.Key, " = ", property.Value, ";" }));
                        }
                        else if ((type == typeof(int[])) || (type == typeof(float[])) || (type == typeof(Vector3)))
                        {
                            stringBuilder.AppendLine(string.Concat(new string[] { str6, ".", property.Key, " = { ", property.Value.Replace(" ", ", "), " };" }));
                        }
                    }
                }
                stringBuilder.AppendLine("");
                num++;
            }
            stringBuilder.AppendLine(string.Concat("WLD_Save(\"", str5, "\");"));
            stringBuilder.AppendLine("}");
            File.WriteAllText(str3, stringBuilder.ToString());
            Process zSlangExe = new Process();
            zSlangExe.StartInfo.FileName = this.ZSlangExe;
            zSlangExe.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.ZSlangExe);
            zSlangExe.StartInfo.Arguments = Helper.EscapeArgument(str3);
            zSlangExe.Start();
            zSlangExe.WaitForExit();
            try
            {
                TestStack.White.Application application = TestStack.White.Application.Attach("zSpy");
                Window window2 = application.GetWindow("[zSpy]");
                Clipboard.Clear();
                window2.Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);
                window2.Keyboard.Enter("C");
                window2.Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
                if (Clipboard.GetText(TextDataFormat.Text).Contains("Execution finished normally"))
                {
                    application.Kill();
                }
            }
            catch
            {
            }
        }
    }
}