using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;
using ZenVis.Shared;

namespace ZenVis.Lib
{
    public class ZenVisExp : ZenVis
    {
        public string GothicSourcerExe;

        public string ExportAs;

        public ObjectMergeMode MergeObjects;

        public bool MergeFiles;

        public bool SkipInvisible;

        public bool ExportLevel;

        public ZenVisExp(string appDir, string gothicDir, string gothicVersion, string blenderExe, string outputDir, string gothicSourcerExe, string exportAs, ObjectMergeMode mergeObjects, bool mergeFiles, bool skipInvisible, bool exportLevel) : base(appDir, gothicDir, gothicVersion, blenderExe, outputDir)
        {
            this.GothicSourcerExe = gothicSourcerExe;
            this.ExportAs = exportAs;
            this.MergeObjects = mergeObjects;
            this.MergeFiles = mergeFiles;
            this.SkipInvisible = skipInvisible;
            this.ExportLevel = exportLevel;
            Directory.CreateDirectory(string.Concat(this.AppDir, "OBJ\\Gothic\\Decals"));
            Directory.CreateDirectory(string.Concat(this.AppDir, "OBJ\\Gothic 2\\Decals"));
            Directory.CreateDirectory(string.Concat(this.AppDir, "OBJ\\Mod\\Decals"));
            Directory.CreateDirectory(string.Concat(this.GothicDir, "_WORK\\DATA\\ANIMS\\_COMPILED"));
            Directory.CreateDirectory(string.Concat(this.GothicDir, "_WORK\\DATA\\ANIMS\\MDS_MOBSI"));
            Directory.CreateDirectory(string.Concat(this.GothicDir, "_WORK\\DATA\\ANIMS\\asc_static"));
            Directory.CreateDirectory(string.Concat(this.GothicDir, "_WORK\\DATA\\MESHES\\_COMPILED"));
            Directory.CreateDirectory(string.Concat(this.GothicDir, "_WORK\\DATA\\WORLDS"));
        }

        public void ConvertAnimsToAsc(SortedSet<string> toConvert = null)
        {
            KeyboardInput.SpecialKeys specialKey;
            string[] files = Helper.GetFiles(string.Concat(this.GothicDir, "_WORK\\DATA\\ANIMS\\"), "_COMPILED\\*.*|MDS_MOBSI\\*.*", SearchOption.AllDirectories);
            TestStack.White.Application application = TestStack.White.Application.Launch(this.GothicSourcerExe);
            Window window = application.GetWindows().First<Window>();
            Helper.ResetWindow(window.Title);
            string[] strArrays = files;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                string ext = Path.GetExtension(str).ToUpper();

                if (ext == ".MDL")
                    specialKey = KeyboardInput.SpecialKeys.SHIFT;
                else if ((ext == ".MDS") || (ext == ".MSB"))
                    specialKey = KeyboardInput.SpecialKeys.CONTROL;
                else if (ext == ".MMB")
                    specialKey = KeyboardInput.SpecialKeys.ALT;
                else
                    continue;

                if (!File.Exists(string.Concat(new string[] { this.GothicDir, "_WORK\\DATA\\ANIMS\\asc_", Path.GetFileNameWithoutExtension(str).ToLower(), "\\", Path.GetFileNameWithoutExtension(str).ToUpper(), ".ASC" })))
                {
                    if (!File.Exists(string.Concat(new string[] { this.GothicDir, "_WORK\\DATA\\ANIMS\\asc_", Path.GetFileNameWithoutExtension(str).ToLower(), "\\", Path.GetFileNameWithoutExtension(str).ToUpper(), ".MDS" })) && !File.Exists(string.Concat(this.GothicDir, "_WORK\\DATA\\ANIMS\\asc_static\\", Path.GetFileNameWithoutExtension(str).ToUpper(), ".ASC")))
                    {
                        if (toConvert == null || toConvert.Contains(Path.GetFileNameWithoutExtension(str).ToUpper()))
                        {
                            while (application.GetWindows().Count == 1)
                            {
                                window.Keyboard.HoldKey(specialKey);
                                window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.F8);
                                window.Keyboard.LeaveKey(specialKey);
                            }
                            window.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("1047")).Text = str;
                            window.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("1048")).Text = string.Concat(this.GothicDir, "_WORK\\DATA\\");
                            window.Get<TestStack.White.UIItems.ListBoxItems.ComboBox>(SearchCriteria.ByAutomationId("1051")).Select(this.GothicVersion);
                            try
                            {
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("1")).Click();
                            }
                            catch
                            {
                            }
                            string text = "";
                            do
                            {
                                text = window.Get<TestStack.White.UIItems.ListView>(SearchCriteria.ByAutomationId("1001")).Rows.Last<ListViewRow>().Cells.First<ListViewCell>().Text;
                                Thread.Sleep(100);
                            }
                            while (!text.Contains("error(s)") || !text.Contains("warning(s)"));
                        }
                    }
                }
            }
            application.Kill();
        }

        public void ConvertAnimsToObj(SortedSet<string> toConvert = null)
        {
            this.ConvertAnimsToAsc(toConvert);
            this.ConvertAscToObj(toConvert);
        }

        public void ConvertAscToObj(SortedSet<string> toConvert = null)
        {
            string[] files = Directory.GetFiles(string.Concat(this.GothicDir, "_WORK\\DATA\\ANIMS\\"), "*.ASC", SearchOption.AllDirectories);
            Regex regex = new Regex("(\\s*)\\*TM_POS\\s(.*)");
            string[] strArrays = files;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                string str1 = string.Concat(new string[] { this.AppDir, "OBJ\\", "Mod", "\\", Path.GetFileNameWithoutExtension(str), ".OBJ" });
                string str2 = string.Concat(new string[] { this.AppDir, "OBJ\\", "Mod", "\\", Path.GetFileNameWithoutExtension(str), ".MTL" });
                if (!File.Exists(str1) && (toConvert == null || toConvert.Contains(Path.GetFileNameWithoutExtension(str).ToUpper())))
                {
                    string[] strArrays1 = File.ReadAllLines(str);
                    Vector3 vector3 = new Vector3(0f, 0f, 0f);
                    int num = 0;
                    while (num < (int)strArrays1.Length)
                    {
                        if (!regex.IsMatch(strArrays1[num]))
                        {
                            num++;
                        }
                        else
                        {
                            string[] strArrays2 = (
                                from a in regex.Split(strArrays1[num])
                                where !string.IsNullOrWhiteSpace(a)
                                select a).ToArray<string>()[0].ToUpper().Split(new char[] { '\t' });
                            vector3.X = Helper.StringToFloat(strArrays2[0]);
                            vector3.Y = Helper.StringToFloat(strArrays2[1]);
                            vector3.Z = Helper.StringToFloat(strArrays2[2]);
                            vector3 /= 100f;
                            break;
                        }
                    }
                    Process process = new Process();
                    process.StartInfo.FileName = this.BlenderExe;
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.BlenderExe);
                    process.StartInfo.Arguments = string.Concat(new string[] { Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\Empty.blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\convert_asc_to_obj.py")), " -- ", Helper.EscapeArgument(str), " ", Helper.EscapeArgument(str1), " ", Helper.EscapeArgument(Helper.FloatToString(vector3.X)), " ", Helper.EscapeArgument(Helper.FloatToString(vector3.Y)), " ", Helper.EscapeArgument(Helper.FloatToString(vector3.Z)) });
                    process.Start();
                    while (!process.HasExited && Process.GetProcessesByName("wxImpExpUI").Length == 0)
                    {
                        Thread.Sleep(1000);
                    }
                    if (!process.HasExited)
                    {
                        Window window = TestStack.White.Application.Attach("wxImpExpUI").GetWindow("Kerrax ASCII Model Importer");
                        Helper.ResetWindow(window.Title);
                        string name = window.Get<Tab>(SearchCriteria.ByAutomationId("-207")).SelectedTab.Name;
                        if (name == "Morph Animation" || name == "Dynamic Model Animation")
                        {
                            window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-330")).Click();
                            process.WaitForExit();
                        }
                        else
                        {
                            if (name == "Static Model")
                            {
                                window.Get<TestStack.White.UIItems.RadioButton>(SearchCriteria.ByAutomationId("-259")).Click();
                                window.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-264")).Text = "";
                                window.Get<TestStack.White.UIItems.CheckBox>(SearchCriteria.ByAutomationId("-269")).Checked = false;
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-267")).Click();
                                Window window1 = window.ModalWindow("Space Transformation");
                                window1.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-341")).Text = "0.01";
                                window1.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-342")).Click();
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-329")).Click();
                            }
                            else if (name != "Dynamic Model")
                            {
                                if (name != "Morph Mesh")
                                {
                                    throw new AutomationException("Unkown File Type", "This file type is unknown, therefore ZenVis can't automate KrxImpExp!");
                                }
                                window.Get<TestStack.White.UIItems.RadioButton>(SearchCriteria.ByAutomationId("-217")).Click();
                                window.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-228")).Text = "";
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-231")).Click();
                                Window window2 = window.ModalWindow("Space Transformation");
                                try
                                {
                                    window2.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-341")).Text = "0.01";
                                    window2.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-342")).Click();
                                }
                                catch
                                {
                                    window2.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-363")).Text = "0.01";
                                    window2.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-364")).Click();
                                }
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-329")).Click();
                            }
                            else
                            {
                                window.Get<TestStack.White.UIItems.RadioButton>(SearchCriteria.ByAutomationId("-283")).Click();
                                window.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-288")).Text = "";
                                window.Get<TestStack.White.UIItems.CheckBox>(SearchCriteria.ByAutomationId("-295")).Checked = true;
                                window.Get<TestStack.White.UIItems.CheckBox>(SearchCriteria.ByAutomationId("-296")).Checked = false;
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-293")).Click();
                                Window window3 = window.ModalWindow("Space Transformation");
                                window3.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-341")).Text = "0.01";
                                window3.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-342")).Click();
                                window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-329")).Click();
                            }
                            process.WaitForExit();
                            this.FixObjAndMtl(str, str1, str2);
                        }
                    }
                }
            }
        }

        public void ConvertMeshesToObj(SortedSet<string> toConvert = null)
        {
            string[] array = (
                from y in Directory.GetFiles(string.Concat(this.GothicDir, "_WORK\\DATA\\MESHES\\_COMPILED\\"), "*.MRM", SearchOption.AllDirectories)
                select y.ToUpper()).ToArray<string>();
            for (int i = 0; i < (int)array.Length; i++)
            {
                string str = array[i];
                string str1 = string.Concat(new string[] { this.AppDir, "OBJ\\", "Mod", "\\", Path.GetFileNameWithoutExtension(str), ".OBJ" });
                string str2 = string.Concat(new string[] { this.AppDir, "OBJ\\", "Mod", "\\", Path.GetFileNameWithoutExtension(str), ".MTL" });
                if (!File.Exists(str1) && (toConvert == null || toConvert.Contains(Path.GetFileNameWithoutExtension(str).ToUpper())))
                {
                    Process process = new Process();
                    process.StartInfo.FileName = this.BlenderExe;
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.BlenderExe);
                    process.StartInfo.Arguments = string.Concat(new string[] { Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\Empty.blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\convert_mrm_to_obj.py")), " -- ", Helper.EscapeArgument(Path.GetFileNameWithoutExtension(str)), " ", Helper.EscapeArgument(str), " ", Helper.EscapeArgument(str1) });
                    process.Start();
                    while (!process.HasExited && Process.GetProcessesByName("wxImpExpUI").Length == 0)
                    {
                        Thread.Sleep(100);
                    }
                    if (!process.HasExited)
                    {
                        Window window = TestStack.White.Application.Attach("wxImpExpUI").GetWindow("Kerrax Multi-Resolution Mesh Importer");
                        Helper.ResetWindow(window.Title);
                        window.Get<TestStack.White.UIItems.RadioButton>(SearchCriteria.ByAutomationId("-208")).Click();
                        window.Get<TestStack.White.UIItems.CheckBox>(SearchCriteria.ByAutomationId("-214")).Checked = false;
                        window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-211")).Click();
                        Window window1 = window.ModalWindow("Space Transformation");
                        window1.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-230")).Text = "0.01";
                        window1.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-231")).Click();
                        window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-218")).Click();
                        process.WaitForExit();
                        this.FixObjAndMtl(str, str1, str2);
                    }
                }
            }
        }

        public void Export(string[] zenFiles)
        {
            int i;
            if (Directory.Exists(string.Concat(this.GothicDir, "_WORK\\DATA\\SCRIPTS\\CONTENT\\ITEMS")))
            {
                try
                {
                    foreach (KeyValuePair<string, string> instancesFromScript in Item.GetInstancesFromScripts(this.GothicDir))
                    {
                        if (!Item.Instances[this.GothicVersion].ContainsKey(instancesFromScript.Key))
                        {
                            Item.Instances[this.GothicVersion].Add(instancesFromScript.Key, instancesFromScript.Value);
                        }
                        else
                        {
                            Item.Instances[this.GothicVersion][instancesFromScript.Key] = instancesFromScript.Value;
                        }
                    }
                }
                catch
                {
                }
            }
            for (i = 0; i < (int)zenFiles.Length; i++)
            {
                try
                {
                    if (!this.IsZenBinarySafe(zenFiles[i]))
                    {
                        List<Visual> visualsFromZen = this.GetVisualsFromZen(zenFiles[i]);
                        if (visualsFromZen.Count<Visual>() != 0)
                        {
                            bool flag = false;
                            do
                            {
                                SortedSet<string> missingVisuals = this.GetMissingVisuals(visualsFromZen);
                                if (missingVisuals.Count<string>() <= 0)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    MessageBox.Show(string.Format(Localization.Instance.GetTranslation("ZenMissingVisuals", null), Path.GetFileName(zenFiles[i]), string.Join(", ", missingVisuals)), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    if (MessageBox.Show(Localization.Instance.GetTranslation("ConverterHint", null), "ZenVis", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        this.ConvertAnimsToObj(missingVisuals);
                                        this.ConvertMeshesToObj(missingVisuals);
                                    }
                                    else if (MessageBox.Show(Localization.Instance.GetTranslation("ExportAnyway", null), "ZenVis", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                                    {
                                        zenFiles[i] = "";
                                    }
                                    else
                                    {
                                        visualsFromZen = (
                                            from x in visualsFromZen
                                            where !missingVisuals.Contains(x.FileName)
                                            select x).ToList<Visual>();
                                        flag = true;
                                    }
                                }
                            }
                            while (!flag && zenFiles[i] != "");

                            if (zenFiles[i] != "")
                            {
                                this.ImportObjAndSave(zenFiles[i], visualsFromZen);
                                if (!File.Exists(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zenFiles[i]), ".blend")))
                                {
                                    MessageBox.Show(string.Format(Localization.Instance.GetTranslation("BlenderUnknownError", null), string.Concat(Path.GetFileNameWithoutExtension(zenFiles[i]), ".blend")), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(string.Format(Localization.Instance.GetTranslation("ZenNoVisual", null), Path.GetFileName(zenFiles[i])), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Localization.Instance.GetTranslation("ZenIsBinarySafe", null), Path.GetFileName(zenFiles[i])), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString(), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }

            if (zenFiles.Any(z => z != ""))
            {
                if (!this.MergeFiles)
                {
                    for (i = 0; i < (int)zenFiles.Length; i++)
                    {
                        if (zenFiles[i] == "")
                            continue;
                        this.ExportBlend(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zenFiles[i]), ".blend"));
                    }
                    return;
                }
                this.MergeBlends(zenFiles, "ZenVisExp.blend");
                for (i = 0; i < (int)zenFiles.Length; i++)
                {
                    if (zenFiles[i] == "")
                        continue;
                    if (File.Exists(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zenFiles[i]), ".blend")))
                    {
                        File.Delete(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zenFiles[i]), ".blend"));
                    }
                }
                this.ExportBlend(string.Concat(this.OutputDir, "ZenVisExp.blend"));
            }
        }

        public void ExportBlend(string blend)
        {
            if (!File.Exists(blend) || this.ExportAs == "BLEND")
            {
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("import bpy");
            stringBuilder.AppendLine("");
            if (this.ExportAs == "DAE")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.wm.collada_export(filepath='", string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(blend), ".dae").Replace("\\", "/"), "', check_existing = False)"));
            }
            else if (this.ExportAs == "FBX")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.export_scene.fbx(filepath='", string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(blend), ".fbx").Replace("\\", "/"), "', check_existing = False)"));
            }
            else if (this.ExportAs == "OBJ")
            {
                stringBuilder.AppendLine(string.Concat("bpy.ops.export_scene.obj(filepath='", string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(blend), ".obj").Replace("\\", "/"), "', check_existing = False, path_mode = 'STRIP')"));
            }
            File.WriteAllText(string.Concat(this.AppDir, "Blender\\export_blend.py"), stringBuilder.ToString());
            Process process = new Process();
            process.StartInfo.FileName = this.BlenderExe;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.BlenderExe);
            process.StartInfo.Arguments = string.Concat(Helper.EscapeArgument(blend), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\export_blend.py")));
            process.Start();
            process.WaitForExit();
        }

        public void FixObjAndMtl(string file, string obj, string mtl)
        {
            if (File.Exists(obj))
            {
                string[] strArrays = File.ReadAllLines(obj);
                int num = 0;
                while (num < (int)strArrays.Length)
                {
                    if (!strArrays[num].StartsWith("o "))
                    {
                        num++;
                    }
                    else
                    {
                        strArrays[num] = string.Concat("o ", Path.GetFileNameWithoutExtension(file).ToUpper());
                        break;
                    }
                }
                File.WriteAllLines(obj, strArrays);
            }
            if (File.Exists(mtl))
            {
                File.Move(mtl, mtl);
            }
        }

        public void GenerateObjForDecal(Visual visual)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Concat("mtllib ", visual.FileName, ".MTL"));
            stringBuilder.AppendLine(string.Concat("o ", visual.FileName));
            stringBuilder.AppendLine("v -1.000000 1.000000 0.000000");
            stringBuilder.AppendLine("v 1.000000 1.000000 0.000000");
            stringBuilder.AppendLine("v 1.000000 -1.000000 0.000000");
            stringBuilder.AppendLine("v -1.000000 -1.000000 0.000000");
            stringBuilder.AppendLine("vt 0.000000 0.000000");
            stringBuilder.AppendLine("vt 1.000000 0.000000");
            stringBuilder.AppendLine("vt 1.000000 1.000000");
            stringBuilder.AppendLine("vt 0.000000 1.000000");
            stringBuilder.AppendLine("vn 0.000000 0.000000 -1.000000");
            stringBuilder.AppendLine(string.Concat("usemtl ", visual.FileName));
            stringBuilder.AppendLine("s off");
            stringBuilder.AppendLine("f 1/1/1 2/2/1 3/3/1 4/4/1");
            File.WriteAllText(string.Concat(new string[] { this.AppDir, "OBJ\\", "Mod", "\\Decals\\", visual.FileName, ".OBJ" }), stringBuilder.ToString());
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendLine(string.Concat("newmtl ", visual.FileName));
            stringBuilder1.AppendLine("Ns 96.078431");
            stringBuilder1.AppendLine("Ka 0.000000 0.000000 0.000000");
            stringBuilder1.AppendLine("Kd 0.640000 0.640000 0.640000");
            stringBuilder1.AppendLine("Ks 0.500000 0.500000 0.500000");
            stringBuilder1.AppendLine("Ni 1.000000");
            stringBuilder1.AppendLine("d 1.000000");
            stringBuilder1.AppendLine("illum 2");
            stringBuilder1.AppendLine(string.Concat("map_Kd ", visual.FileName, ".TGA"));
            File.WriteAllText(string.Concat(new string[] { this.AppDir, "OBJ\\", "Mod", "\\Decals\\", visual.FileName, ".MTL" }), stringBuilder1.ToString());
        }

        public SortedSet<string> GetMissingVisuals(List<Visual> visuals)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(string.Concat(this.AppDir, "OBJ\\", this.GothicVersion), "*.OBJ", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(string.Concat(this.AppDir, "OBJ\\", "Mod"), "*.OBJ", SearchOption.AllDirectories));
            string[] array = files.Select(f => Path.GetFileName(f).ToUpper()).ToArray();

            SortedSet<string> strs = new SortedSet<string>();
            foreach (Visual visual in visuals)
            {
                if (visual.GetType() == typeof(Decal) || visual.GetType() == typeof(Level) || array.Contains<string>(string.Concat(visual.FileName, ".OBJ")))
                {
                    continue;
                }
                strs.Add(visual.FileName);
            }
            return strs;
        }

        public List<Visual> GetVisualsFromZen(string zen)
        {
            string[] strArrays = File.ReadAllLines(zen);
            List<Visual> visuals = new List<Visual>();
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                if (Parser.RegVisualType.IsMatch(strArrays[i]))
                {
                    string str = strArrays[i].Substring(0, strArrays[i].Length - strArrays[i].TrimStart(Array.Empty<char>()).Length);
                    int num = i;
                    while (num < (int)strArrays.Length)
                    {
                        if (strArrays[num] != string.Concat(str, "[]"))
                        {
                            num++;
                        }
                        else
                        {
                            string[] strArrays1 = strArrays.SubArray<string>(i, num - i + 1);
                            Visual visual = VisualFactory.GetVisual(this.GothicDir, this.GothicVersion, ref strArrays1);
                            if (visual == null || this.SkipInvisible && !visual.Visible)
                            {
                                break;
                            }
                            visuals.Add(visual);
                            break;
                        }
                    }
                }
            }
            return (
                from x in visuals
                orderby x.FileName
                select x).ToList<Visual>();
        }

        public void ImportObjAndSave(string zen, List<Visual> visuals)
        {
            Dictionary<string, List<Visual>> strs = new Dictionary<string, List<Visual>>();
            foreach (Visual visual in visuals)
            {
                string str = string.Concat(Path.GetFileNameWithoutExtension(zen), "_", visual.FileName);
                if (!strs.ContainsKey(str))
                {
                    strs.Add(str, new List<Visual>());
                }
                strs[str].Add(visual);
            }
            string fileName = "";
            string str1 = "";
            int num = 0;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("import bpy");
            stringBuilder.AppendLine("import mathutils");
            stringBuilder.AppendLine("import sys");
            stringBuilder.AppendLine(string.Concat("sys.path.append('", string.Concat(this.AppDir, "Blender").Replace("\\", "/"), "')"));
            stringBuilder.AppendLine("from zen_vis import *");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("import logging");
            stringBuilder.AppendLine("import traceback");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("def log_to_file(etype, value, tb):");
            stringBuilder.AppendLine("  file = open('" + (Helper.ApplicationDirectory() + "\\ZenVis.txt").Replace("\\", "/") + "', 'a')");
            stringBuilder.AppendLine("  file.write(''.join(traceback.format_exception(etype, value, tb)))");
            stringBuilder.AppendLine("  file.close()");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("sys.excepthook = log_to_file");
            stringBuilder.AppendLine("");
            foreach (List<Visual> value in strs.Values)
            {
                foreach (Visual visual1 in value)
                {
                    if (visual1.GetType() == typeof(Level))
                    {
                        continue;
                    }
                    if (fileName != visual1.FileName)
                    {
                        fileName = visual1.FileName;
                        num = 0;
                    }
                    str1 = string.Concat(visual1.FileName, (num == 0 ? "" : string.Concat(".", string.Format("{0:000}", num))));
                    if (visual1 != value.First<Visual>())
                    {
                        stringBuilder.AppendLine("bpy.ops.object.duplicate(linked=True)");
                    }
                    else
                    {
                        if (visual1.GetType() == typeof(Decal))
                        {
                            this.GenerateObjForDecal(visual1);
                        }
                        StringBuilder stringBuilder1 = stringBuilder;
                        string[] appDir = new string[] { this.AppDir, "OBJ\\", this.GothicVersion, "\\", null, null, null };
                        appDir[4] = (visual1.GetType() == typeof(Decal) ? "Decals\\" : "");
                        appDir[5] = visual1.FileName;
                        appDir[6] = ".OBJ";

                        if (!File.Exists(string.Concat(appDir)))
                        {
                            appDir[2] = "Mod";
                        }

                        stringBuilder1.AppendLine(string.Concat("bpy.ops.import_scene.obj(filepath='", string.Concat(appDir).Replace("\\", "/"), "', use_image_search=False)"));
                        stringBuilder.AppendLine(string.Concat("clean_object('", str1, "')"));
                        stringBuilder.AppendLine("bpy.ops.object.select_all(action='DESELECT')");
                        stringBuilder.AppendLine(string.Concat("bpy.data.objects['", str1, "'].select = True"));
                    }
                    stringBuilder.AppendLine(string.Concat(new string[] { "bpy.data.objects['", str1, "'].matrix_world = mathutils.Matrix(((", Helper.FloatToString(visual1.Rotation.m[0, 0]), ", ", Helper.FloatToString(visual1.Rotation.m[0, 1]), ", ", Helper.FloatToString(visual1.Rotation.m[0, 2]), ", ", Helper.FloatToString(visual1.Position.X), "), (", Helper.FloatToString(visual1.Rotation.m[2, 0]), ", ", Helper.FloatToString(visual1.Rotation.m[2, 1]), ", ", Helper.FloatToString(visual1.Rotation.m[2, 2]), ", ", Helper.FloatToString(visual1.Position.Z), "), (", Helper.FloatToString(visual1.Rotation.m[1, 0]), ", ", Helper.FloatToString(visual1.Rotation.m[1, 1]), ", ", Helper.FloatToString(visual1.Rotation.m[1, 2]), ", ", Helper.FloatToString(visual1.Position.Y), "), (0, 0, 0, 1)))" }));
                    if (visual1.GetType() != typeof(Decal))
                    {
                        stringBuilder.AppendLine(string.Concat(new object[] { "bpy.data.objects['", str1, "'].scale = mathutils.Vector((", (int)visual1.Scale.X, ", ", (int)visual1.Scale.Y, ", ", (int)visual1.Scale.Z, "))" }));
                    }
                    else
                    {
                        stringBuilder.AppendLine(string.Concat(new string[] { "bpy.data.objects['", str1, "'].dimensions = mathutils.Vector((", Helper.FloatToString(((Decal)visual1).Dimensions.X), ", ", Helper.FloatToString(((Decal)visual1).Dimensions.Y), ", ", Helper.FloatToString(((Decal)visual1).Dimensions.Z), "))" }));
                    }
                    if (visual1 == value.Last<Visual>())
                    {
                        if (MergeObjects == ObjectMergeMode.Name)
                        {
                            stringBuilder.AppendLine("bpy.ops.object.select_all(action='SELECT')");
                            stringBuilder.AppendLine("bpy.context.scene.objects.active = bpy.data.objects[0]");
                            stringBuilder.AppendLine("bpy.ops.object.join()");
                            stringBuilder.AppendLine("bpy.data.objects[0].name = '" + visual1.FileName + "'");
                            stringBuilder.AppendLine("bpy.data.objects['" + visual1.FileName + "'].data.name = '" + visual1.FileName + "'");
                        }
                        stringBuilder.AppendLine(string.Concat("bpy.ops.wm.save_as_mainfile(filepath='", string.Concat(new string[] { this.OutputDir, Path.GetFileNameWithoutExtension(zen), "_", fileName, ".blend" }).Replace("\\", "/"), "', check_existing=False)"));
                        stringBuilder.AppendLine(string.Concat("bpy.ops.wm.open_mainfile(filepath='", string.Concat(this.AppDir, "Blender\\Empty.blend").Replace("\\", "/"), "')"));
                    }
                    num++;
                }
            }
            File.WriteAllText(string.Concat(this.AppDir, "Blender\\import_obj_and_save_as_blends.py"), stringBuilder.ToString());
            stringBuilder.Clear();
            Process process = new Process();
            process.StartInfo.FileName = this.BlenderExe;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.BlenderExe);
            process.StartInfo.Arguments = string.Concat(Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\Empty.blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\import_obj_and_save_as_blends.py")));
            process.Start();
            process.WaitForExit();

            List<string> mergeFiles = strs.Where(x => x.Value.All(y => y.GetType() != typeof(Level))).Select(x => x.Key).ToList();

            if (this.ExportLevel && Directory.Exists(this.GothicDir + "_WORK\\DATA\\MESHES"))
            {
                foreach (List<Visual> value in strs.Values)
                {
                    foreach (Visual visual1 in value)
                    {
                        if (visual1.GetType() == typeof(Level))
                        {
                            string[] files = Directory.GetFiles(this.GothicDir + "_WORK\\DATA\\MESHES", visual1.FileName + ".3DS", SearchOption.AllDirectories);

                            if (files.Length > 0)
                            {
                                string blendFile = (OutputDir + Path.GetFileNameWithoutExtension(zen) + "_" + visual1.FileName + ".blend").Replace("\\", "/");
                                stringBuilder.AppendLine("import bpy");
                                stringBuilder.AppendLine("import re");
                                stringBuilder.AppendLine("import os");
                                stringBuilder.AppendLine("import sys");
                                stringBuilder.AppendLine("sys.path.append('" + (this.AppDir + "Blender").Replace("\\", "/") + "')");
                                stringBuilder.AppendLine("from zen_vis import *");
                                stringBuilder.AppendLine("");
                                stringBuilder.AppendLine("import logging");
                                stringBuilder.AppendLine("import traceback");
                                stringBuilder.AppendLine("");
                                stringBuilder.AppendLine("def log_to_file(etype, value, tb):");
                                stringBuilder.AppendLine("  file = open('" + (Helper.ApplicationDirectory() + "\\ZenVis.txt").Replace("\\", "/") + "', 'a')");
                                stringBuilder.AppendLine("  file.write(''.join(traceback.format_exception(etype, value, tb)))");
                                stringBuilder.AppendLine("  file.close()");
                                stringBuilder.AppendLine("");
                                stringBuilder.AppendLine("sys.excepthook = log_to_file");
                                stringBuilder.AppendLine("");
                                stringBuilder.AppendLine("bpy.ops.import_scene.krx3dsimp(filepath='" + files[0].Replace("\\", "/") + "')");
                                stringBuilder.AppendLine("print(\"Cleaning..\")");
                                stringBuilder.AppendLine("prepare_scene()");
                                stringBuilder.AppendLine("regex = re.compile(r'_zenGin([0-9]*)([\\.]*)([0-9]*)', re.IGNORECASE)");
                                stringBuilder.AppendLine("for obj in bpy.data.objects:");
                                stringBuilder.AppendLine("  clean_object(obj.name)");
                                stringBuilder.AppendLine("  obj.name = regex.sub('', obj.name)");
                                stringBuilder.AppendLine("  obj.data.name = regex.sub('', obj.data.name)");
                                stringBuilder.AppendLine("bpy.ops.wm.save_as_mainfile(filepath='" + blendFile + "', check_existing=False)");
                                stringBuilder.AppendLine("bpy.ops.wm.open_mainfile(filepath='" + blendFile + "')");
                                stringBuilder.AppendLine("remove_unused_images()");
                                stringBuilder.AppendLine("bpy.ops.wm.save_as_mainfile(filepath='" + blendFile + "', check_existing=False)");

                                File.WriteAllText(string.Concat(this.AppDir, "Blender\\import_level_from_3ds.py"), stringBuilder.ToString());
                                stringBuilder.Clear();

                                process.StartInfo.Arguments = string.Concat(Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\Empty.blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\import_level_from_3ds.py")));
                                process.Start();

                                while (!process.HasExited && Process.GetProcessesByName("wxImpExpUI").Length == 0)
                                {
                                    Thread.Sleep(1000);
                                }

                                if (!process.HasExited)
                                {
                                    Window window = TestStack.White.Application.Attach("wxImpExpUI").GetWindow("Kerrax 3D Studio Mesh Importer");
                                    Helper.ResetWindow(window.Title);
                                    window.Get<TestStack.White.UIItems.RadioButton>(SearchCriteria.ByAutomationId("-209")).Click();
                                    window.Get<TestStack.White.UIItems.CheckBox>(SearchCriteria.ByAutomationId("-225")).Checked = false;
                                    window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-217")).Click();
                                    Window window1 = window.ModalWindow("Space Transformation");
                                    window1.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("-241")).Text = "0.01";
                                    window1.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-242")).Click();
                                    window.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("-229")).Click();
                                    process.WaitForExit();

                                    mergeFiles.Add(Path.GetFileNameWithoutExtension(zen) + "_" + visual1.FileName);
                                }
                            }
                        }
                    }
                }
            }

            this.MergeBlends(mergeFiles.ToArray(), string.Concat(Path.GetFileNameWithoutExtension(zen), ".blend"));
            foreach (string file in mergeFiles)
            {
                if (File.Exists(string.Concat(this.OutputDir, file, ".blend")))
                    File.Delete(string.Concat(this.OutputDir, file, ".blend"));
                if (File.Exists(string.Concat(this.OutputDir, file, ".blend1")))
                    File.Delete(string.Concat(this.OutputDir, file, ".blend1"));

            }
            if (MergeObjects == ObjectMergeMode.All)
            {
                stringBuilder.Clear();
                stringBuilder.AppendLine("import bpy");
                stringBuilder.AppendLine("import mathutils");
                stringBuilder.AppendLine("import sys");
                stringBuilder.AppendLine("sys.path.append('" + (this.AppDir + "Blender").Replace("\\", "/") + "')");
                stringBuilder.AppendLine("from zen_vis import *");
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("bpy.ops.object.select_all(action='SELECT')");
                stringBuilder.AppendLine("bpy.context.scene.objects.active = bpy.data.objects[0]");
                stringBuilder.AppendLine("bpy.ops.object.join()");
                stringBuilder.AppendLine("bpy.data.objects[0].name = '" + Path.GetFileNameWithoutExtension(zen) + "'");
                stringBuilder.AppendLine("bpy.data.objects['" + Path.GetFileNameWithoutExtension(zen) + "'].data.name = '" + Path.GetFileNameWithoutExtension(zen) + "'");
                stringBuilder.AppendLine("bpy.ops.wm.save_as_mainfile(filepath='" + (this.OutputDir + Path.GetFileNameWithoutExtension(zen) + ".blend").Replace("\\", "/") + "', check_existing=False)");
                File.WriteAllText(string.Concat(this.AppDir, "Blender\\merge_objects.py"), stringBuilder.ToString());
                process.StartInfo.Arguments = string.Concat(Helper.EscapeArgument(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zen), ".blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\merge_objects.py")));
                process.Start();
                process.WaitForExit();
                if (File.Exists(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zen), ".blend1")))
                {
                    File.Delete(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(zen), ".blend1"));
                }
            }
        }

        public bool IsZenBinarySafe(string zen)
        {
            string[] strArrays = File.ReadAllLines(zen);
            for (int i = 0; strArrays[i] != "END"; i++)
            {
                if (strArrays[i] == "zCArchiverBinSafe")
                {
                    return true;
                }
            }
            return false;
        }

        public void MergeBlends(string[] files, string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("import bpy");
            stringBuilder.AppendLine("import sys");
            stringBuilder.AppendLine(string.Concat("sys.path.append('", string.Concat(this.AppDir, "Blender").Replace("\\", "/"), "')"));
            stringBuilder.AppendLine("from zen_vis import *");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("import logging");
            stringBuilder.AppendLine("import traceback");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("def log_to_file(etype, value, tb):");
            stringBuilder.AppendLine("  file = open('" + (Helper.ApplicationDirectory() + "\\ZenVis.txt").Replace("\\", "/") + "', 'a')");
            stringBuilder.AppendLine("  file.write(''.join(traceback.format_exception(etype, value, tb)))");
            stringBuilder.AppendLine("  file.close()");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("sys.excepthook = log_to_file");
            stringBuilder.AppendLine("");
            string[] strArrays = files;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                if (File.Exists(string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(str), ".blend")))
                {
                    stringBuilder.AppendLine(string.Concat("print(\"Appending ", Path.GetFileNameWithoutExtension(str), ".blend..\")"));
                    stringBuilder.AppendLine(string.Concat("with bpy.data.libraries.load(filepath='", string.Concat(this.OutputDir, Path.GetFileNameWithoutExtension(str), ".blend").Replace("\\", "/"), "') as (data_from, data_to):"));
                    stringBuilder.AppendLine("    data_to.objects = data_from.objects");
                    stringBuilder.AppendLine("for obj in data_to.objects:");
                    stringBuilder.AppendLine("    if obj is not None:");
                    stringBuilder.AppendLine("        bpy.context.scene.objects.link(obj)");
                    stringBuilder.AppendLine("        clean_object(obj.name)");
                }
            }
            stringBuilder.AppendLine("bpy.ops.object.select_all(action='DESELECT')");
            stringBuilder.AppendLine(string.Concat("bpy.ops.wm.save_as_mainfile(filepath='", string.Concat(this.OutputDir, fileName).Replace("\\", "/"), "', check_existing = False)"));
            stringBuilder.AppendLine(string.Concat("bpy.ops.wm.open_mainfile(filepath='", string.Concat(this.OutputDir, fileName).Replace("\\", "/"), "')"));
            stringBuilder.AppendLine("print(\"Cleaning..\")");
            stringBuilder.AppendLine("remove_unused_images()");
            stringBuilder.AppendLine(string.Concat("bpy.ops.wm.save_as_mainfile(filepath='", string.Concat(this.OutputDir, fileName).Replace("\\", "/"), "', check_existing=False)"));
            File.WriteAllText(string.Concat(this.AppDir, "Blender\\merge_blends.py"), stringBuilder.ToString());
            Process process = new Process();
            process.StartInfo.FileName = this.BlenderExe;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.BlenderExe);
            process.StartInfo.Arguments = string.Concat(Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\Empty.blend")), " -noaudio --background --python ", Helper.EscapeArgument(string.Concat(this.AppDir, "Blender\\merge_blends.py")));
            process.Start();
            process.WaitForExit();
            if (File.Exists(string.Concat(this.OutputDir, fileName, "1")))
            {
                File.Delete(string.Concat(this.OutputDir, fileName, "1"));
            }
        }
    }
}