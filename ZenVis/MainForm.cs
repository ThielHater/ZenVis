using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ZenVis.Lib;
using ZenVis.Properties;
using ZenVis.Shared;

namespace ZenVis
{
    public class MainForm : Form
    {
        private bool Initialized;

        private IContainer components;

        private TextBox txtGothic1;

        private Button btnGothic1;

        private TextBox txtGothicSourcer;

        private Button btnGothicSourcer;

        private TextBox txtBlender;

        private Button btnBlender;

        private Label lblExportAs;

        private ComboBox cmbExportAs;

        private Button btnExport;

        private CheckBox chbMergeFiles;

        private TextBox txtOutput;

        private Button btnOutput;

        private Button btnImport;

        private TextBox txtZSlang;

        private Button btnZSlang;

        private Label lblGothic1;

        private Label lblBlender;

        private Label lblOutput;

        private TextBox txtGothic2;

        private Button btnGothic2;

        private Label lblGothic2;

        private Label lblVersion;

        private ComboBox cmbVersion;

        private GroupBox grpGeneral;

        private Label lblZSlang;

        private GroupBox grpImport;

        private Label lblGothicSourcer;

        private GroupBox grpExport;

        private CheckBox chbExportLevel;

        private ComboBox cmbMergeObjects;

        private Label lblMergeObjects;

        private CheckBox chbSkipInvisible;

        public MainForm()
        {
            this.InitializeComponent();
            this.Icon = Resources.ZenVis;
            this.grpGeneral.Text = Localization.Instance.GetTranslation("grpGeneral", null);
            this.lblGothic1.Text = Localization.Instance.GetTranslation("lblGothic1", null);
            this.lblGothic2.Text = Localization.Instance.GetTranslation("lblGothic2", null);
            this.lblBlender.Text = Localization.Instance.GetTranslation("lblBlender", null);
            this.lblOutput.Text = Localization.Instance.GetTranslation("lblOutput", null);
            this.lblVersion.Text = Localization.Instance.GetTranslation("lblVersion", null);
            this.cmbVersion.SelectedIndex = 0;
            this.grpImport.Text = Localization.Instance.GetTranslation("grpImport", null);
            this.lblZSlang.Text = Localization.Instance.GetTranslation("lblZSlang", null);
            this.btnImport.Text = Localization.Instance.GetTranslation("btnImport", null);
            this.grpExport.Text = Localization.Instance.GetTranslation("grpExport", null);
            this.lblGothicSourcer.Text = Localization.Instance.GetTranslation("lblGothicSourcer", null);
            this.lblMergeObjects.Text = Localization.Instance.GetTranslation("lblMergeObjects", null);
            this.cmbMergeObjects.Items.Add(Localization.Instance.GetTranslation("MergeModeNone", null));
            this.cmbMergeObjects.Items.Add(Localization.Instance.GetTranslation("MergeModeName", null));
            this.cmbMergeObjects.Items.Add(Localization.Instance.GetTranslation("MergeModeAll", null));
            this.cmbMergeObjects.SelectedIndex = 0;
            this.chbMergeFiles.Text = Localization.Instance.GetTranslation("chbMergeFiles", null);
            this.chbSkipInvisible.Text = Localization.Instance.GetTranslation("chbSkipInvisible", null);
            this.chbExportLevel.Text = Localization.Instance.GetTranslation("chbExportLevel", null);
            this.lblExportAs.Text = Localization.Instance.GetTranslation("lblExportAs", null);
            this.cmbExportAs.SelectedIndex = 0;
            this.btnExport.Text = Localization.Instance.GetTranslation("btnExport", null);
            this.Initialized = false;
            this.txtGothic1.Text = Settings.Default.txtGothic1;
            this.txtGothic2.Text = Settings.Default.txtGothic2;
            this.txtBlender.Text = Settings.Default.txtBlender;
            this.txtOutput.Text = Settings.Default.txtOutput;
            this.cmbVersion.SelectedItem = Settings.Default.cmbVersion;
            this.txtZSlang.Text = Settings.Default.txtZSlang;
            this.txtGothicSourcer.Text = Settings.Default.txtGothicSourcer;
            this.cmbMergeObjects.SelectedIndex = Settings.Default.cmbMergeObjects;
            this.chbMergeFiles.Checked = Settings.Default.chbMergeFiles;
            this.chbSkipInvisible.Checked = Settings.Default.chbSkipInvisible;
            this.chbExportLevel.Checked = Settings.Default.chbExportLevel;
            this.cmbExportAs.SelectedItem = Settings.Default.cmbExportAs;
            this.Initialized = true;
        }

        private void btnBlender_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = Localization.Instance.GetTranslation("ChooseBlenderDirectory", null),
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = (string.IsNullOrEmpty(this.txtBlender.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) : this.txtBlender.Text),
                ShowNewFolderButton = false
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(string.Concat(folderBrowserDialog.SelectedPath, "\\blender.exe")))
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(string.Concat(folderBrowserDialog.SelectedPath, "\\blender.exe"));
                    Version version = new Version(string.Concat(new object[] { versionInfo.ProductMajorPart, ".", versionInfo.ProductMinorPart, ".", versionInfo.ProductBuildPart, ".", versionInfo.ProductPrivatePart }));
                    if (version < new Version("2.7.5"))
                    {
                        MessageBox.Show(Localization.Instance.GetTranslation("BlenderVersionMismatch", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (Directory.Exists(string.Concat(new object[] { folderBrowserDialog.SelectedPath, "\\2.66\\scripts\\addons\\KrxImpExp" })))
                    {
                        if (MessageBox.Show(Localization.Instance.GetTranslation("KrxImpExpNotInstalled"), "ZenVis", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                            return;
                        Helper.CopyDirectory(folderBrowserDialog.SelectedPath + "\\2.66\\scripts\\addons\\KrxImpExp", folderBrowserDialog.SelectedPath + "\\2.75\\scripts\\addons\\KrxImpExp", SearchOption.AllDirectories);
                        Process.Start(folderBrowserDialog.SelectedPath + "\\blender.exe", Helper.EscapeArgument(Helper.ApplicationDirectory() + "\\Blender\\Empty.blend") + " -noaudio --background --python " + Helper.EscapeArgument(Helper.ApplicationDirectory() + "\\Blender\\enable_krximpexp.py"));
                    }
                    if (!Directory.Exists(string.Concat(new object[] { folderBrowserDialog.SelectedPath, "\\", version.Major, ".", version.Minor, version.Build, "\\scripts\\addons\\KrxImpExp" })))
                    {
                        MessageBox.Show(Localization.Instance.GetTranslation("KrxImpExpNotFound", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    this.txtBlender.Text = folderBrowserDialog.SelectedPath;
                    Settings.Default.txtBlender = this.txtBlender.Text;
                    Settings.Default.Save();
                    return;
                }
                MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedItem = (string)this.cmbVersion.SelectedItem;
                string str = (selectedItem == "Gothic" ? this.txtGothic1.Text : this.txtGothic2.Text);
                if (!File.Exists((selectedItem == "Gothic" ? string.Concat(this.txtGothic1.Text, "\\System\\Gothic.exe") : string.Concat(this.txtGothic2.Text, "\\System\\Gothic2.exe"))) || !File.Exists(string.Concat(this.txtBlender.Text, "\\blender.exe")) || !File.Exists(string.Concat(this.txtGothicSourcer.Text, "\\GothicSourcer\\GothicSourcerV3_14.exe")) || !(this.txtOutput.Text != ""))
                {
                    MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        AddExtension = false,
                        AutoUpgradeEnabled = true,
                        CheckFileExists = true,
                        Filter = "ZEN (*.zen)|*.zen",
                        InitialDirectory = str,
                        Multiselect = true,
                        ShowHelp = false,
                        ShowReadOnly = false,
                        SupportMultiDottedExtensions = false,
                        ValidateNames = true
                    };
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        base.Enabled = false;
                        (new ZenVisExp(Helper.ApplicationDirectory(), str, selectedItem, string.Concat(this.txtBlender.Text, "\\blender.exe"), this.txtOutput.Text, string.Concat(this.txtGothicSourcer.Text, "\\GothicSourcer\\GothicSourcerV3_14.exe"), (string)this.cmbExportAs.SelectedItem, (ObjectMergeMode)cmbMergeObjects.SelectedIndex, this.chbMergeFiles.Checked, this.chbSkipInvisible.Checked, this.chbExportLevel.Checked)).Export(openFileDialog.FileNames);
                        base.Enabled = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnGothic1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = Localization.Instance.GetTranslation("ChooseGothic1Directory", null),
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = (string.IsNullOrEmpty(this.txtGothic1.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) : this.txtGothic1.Text),
                ShowNewFolderButton = false
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(string.Concat(folderBrowserDialog.SelectedPath, "\\System\\Gothic.exe")))
                {
                    this.txtGothic1.Text = folderBrowserDialog.SelectedPath;
                    Settings.Default.txtGothic1 = this.txtGothic1.Text;
                    Settings.Default.Save();
                    return;
                }
                MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnGothic2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = Localization.Instance.GetTranslation("ChooseGothic2Directory", null),
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = (string.IsNullOrEmpty(this.txtGothic2.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) : this.txtGothic2.Text),
                ShowNewFolderButton = false
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(string.Concat(folderBrowserDialog.SelectedPath, "\\System\\Gothic2.exe")))
                {
                    this.txtGothic2.Text = folderBrowserDialog.SelectedPath;
                    Settings.Default.txtGothic2 = this.txtGothic2.Text;
                    Settings.Default.Save();
                    return;
                }
                MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnGothicSourcer_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = Localization.Instance.GetTranslation("ChooseGothicSourcerDirectory", null),
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = (string.IsNullOrEmpty(this.txtGothicSourcer.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) : this.txtGothicSourcer.Text),
                ShowNewFolderButton = false
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(string.Concat(folderBrowserDialog.SelectedPath, "\\GothicSourcer\\GothicSourcerV3_14.exe")))
                {
                    if (new Version(FileVersionInfo.GetVersionInfo(string.Concat(folderBrowserDialog.SelectedPath, "\\GothicSourcer\\GothicSourcerV3_14.exe")).ProductVersion.Substring(1)) < new Version("3.14"))
                    {
                        MessageBox.Show(Localization.Instance.GetTranslation("GothicSourcerVersionMismatch", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    this.txtGothicSourcer.Text = folderBrowserDialog.SelectedPath;
                    Settings.Default.txtGothicSourcer = this.txtGothicSourcer.Text;
                    Settings.Default.Save();
                    return;
                }
                MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedItem = (string)this.cmbVersion.SelectedItem;
                string str = (selectedItem == "Gothic" ? this.txtGothic1.Text : this.txtGothic2.Text);
                if (!File.Exists((selectedItem == "Gothic" ? string.Concat(this.txtGothic1.Text, "\\System\\Gothic.exe") : string.Concat(this.txtGothic2.Text, "\\System\\Gothic2.exe"))) || !File.Exists(string.Concat(this.txtBlender.Text, "\\blender.exe")) || !File.Exists(string.Concat(this.txtZSlang.Text, "\\zSlangInterpreter.exe")) || !(this.txtOutput.Text != ""))
                {
                    MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        AddExtension = false,
                        AutoUpgradeEnabled = true,
                        CheckFileExists = true,
                        Filter = "Blender (*.blend)|*.blend|Collada (*.dae)|*.dae|Filmbox (*.fbx)|*.fbx|Wavefront (*.obj)|*.obj",
                        Multiselect = false,
                        RestoreDirectory = true,
                        ShowHelp = false,
                        ShowReadOnly = false,
                        SupportMultiDottedExtensions = false,
                        ValidateNames = true
                    };
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        base.Enabled = false;
                        (new ZenVisImp(Helper.ApplicationDirectory(), str, selectedItem, string.Concat(this.txtBlender.Text, "\\blender.exe"), this.txtOutput.Text, string.Concat(this.txtZSlang.Text, "\\zSlangInterpreter.exe"))).Import(openFileDialog.FileName, false);
                        base.Enabled = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = Localization.Instance.GetTranslation("ChooseOutputDirectory", null),
                RootFolder = Environment.SpecialFolder.Desktop,
                ShowNewFolderButton = true
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtOutput.Text = folderBrowserDialog.SelectedPath;
                Settings.Default.txtOutput = this.txtOutput.Text;
                Settings.Default.Save();
            }
        }

        private void btnZSlang_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = Localization.Instance.GetTranslation("ChooseZSlangDirectory", null),
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = (string.IsNullOrEmpty(this.txtZSlang.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) : this.txtZSlang.Text),
                ShowNewFolderButton = false
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(string.Concat(folderBrowserDialog.SelectedPath, "\\zSlangInterpreter.exe")))
                {
                    this.txtZSlang.Text = folderBrowserDialog.SelectedPath;
                    Settings.Default.txtZSlang = this.txtZSlang.Text;
                    Settings.Default.Save();
                    return;
                }
                MessageBox.Show(Localization.Instance.GetTranslation("InvalidDirectory", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void chbMergeFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            Settings.Default.chbMergeFiles = this.chbMergeFiles.Checked;
            Settings.Default.Save();
        }

        private void chbSkipInvisible_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            Settings.Default.chbSkipInvisible = this.chbSkipInvisible.Checked;
            Settings.Default.Save();
        }

        private void chbExportLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            Settings.Default.chbExportLevel = this.chbExportLevel.Checked;
            Settings.Default.Save();
        }

        private void cmbMergeObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            Settings.Default.cmbMergeObjects = this.cmbMergeObjects.SelectedIndex;
            Settings.Default.Save();
        }

        private void cmbExportAs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            Settings.Default.cmbExportAs = (string)this.cmbExportAs.SelectedItem;
            Settings.Default.Save();
        }

        private void cmbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            Settings.Default.cmbVersion = (string)this.cmbVersion.SelectedItem;
            Settings.Default.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtGothic1 = new System.Windows.Forms.TextBox();
            this.btnGothic1 = new System.Windows.Forms.Button();
            this.txtGothicSourcer = new System.Windows.Forms.TextBox();
            this.btnGothicSourcer = new System.Windows.Forms.Button();
            this.txtBlender = new System.Windows.Forms.TextBox();
            this.btnBlender = new System.Windows.Forms.Button();
            this.chbMergeFiles = new System.Windows.Forms.CheckBox();
            this.lblExportAs = new System.Windows.Forms.Label();
            this.cmbExportAs = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.txtZSlang = new System.Windows.Forms.TextBox();
            this.btnZSlang = new System.Windows.Forms.Button();
            this.lblGothic1 = new System.Windows.Forms.Label();
            this.lblBlender = new System.Windows.Forms.Label();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtGothic2 = new System.Windows.Forms.TextBox();
            this.btnGothic2 = new System.Windows.Forms.Button();
            this.lblGothic2 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.cmbVersion = new System.Windows.Forms.ComboBox();
            this.grpGeneral = new System.Windows.Forms.GroupBox();
            this.lblZSlang = new System.Windows.Forms.Label();
            this.grpImport = new System.Windows.Forms.GroupBox();
            this.lblGothicSourcer = new System.Windows.Forms.Label();
            this.grpExport = new System.Windows.Forms.GroupBox();
            this.cmbMergeObjects = new System.Windows.Forms.ComboBox();
            this.lblMergeObjects = new System.Windows.Forms.Label();
            this.chbExportLevel = new System.Windows.Forms.CheckBox();
            this.chbSkipInvisible = new System.Windows.Forms.CheckBox();
            this.grpGeneral.SuspendLayout();
            this.grpImport.SuspendLayout();
            this.grpExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtGothic1
            // 
            this.txtGothic1.BackColor = System.Drawing.Color.White;
            this.txtGothic1.Enabled = false;
            this.txtGothic1.Location = new System.Drawing.Point(9, 38);
            this.txtGothic1.Name = "txtGothic1";
            this.txtGothic1.Size = new System.Drawing.Size(300, 20);
            this.txtGothic1.TabIndex = 3;
            // 
            // btnGothic1
            // 
            this.btnGothic1.Location = new System.Drawing.Point(315, 37);
            this.btnGothic1.Name = "btnGothic1";
            this.btnGothic1.Size = new System.Drawing.Size(24, 21);
            this.btnGothic1.TabIndex = 4;
            this.btnGothic1.Text = "...";
            this.btnGothic1.UseVisualStyleBackColor = true;
            this.btnGothic1.Click += new System.EventHandler(this.btnGothic1_Click);
            // 
            // txtGothicSourcer
            // 
            this.txtGothicSourcer.BackColor = System.Drawing.Color.White;
            this.txtGothicSourcer.Enabled = false;
            this.txtGothicSourcer.Location = new System.Drawing.Point(9, 38);
            this.txtGothicSourcer.Name = "txtGothicSourcer";
            this.txtGothicSourcer.Size = new System.Drawing.Size(300, 20);
            this.txtGothicSourcer.TabIndex = 21;
            // 
            // btnGothicSourcer
            // 
            this.btnGothicSourcer.Location = new System.Drawing.Point(315, 37);
            this.btnGothicSourcer.Name = "btnGothicSourcer";
            this.btnGothicSourcer.Size = new System.Drawing.Size(24, 21);
            this.btnGothicSourcer.TabIndex = 22;
            this.btnGothicSourcer.Text = "...";
            this.btnGothicSourcer.UseVisualStyleBackColor = true;
            this.btnGothicSourcer.Click += new System.EventHandler(this.btnGothicSourcer_Click);
            // 
            // txtBlender
            // 
            this.txtBlender.BackColor = System.Drawing.Color.White;
            this.txtBlender.Enabled = false;
            this.txtBlender.Location = new System.Drawing.Point(9, 128);
            this.txtBlender.Name = "txtBlender";
            this.txtBlender.Size = new System.Drawing.Size(300, 20);
            this.txtBlender.TabIndex = 9;
            // 
            // btnBlender
            // 
            this.btnBlender.Location = new System.Drawing.Point(315, 127);
            this.btnBlender.Name = "btnBlender";
            this.btnBlender.Size = new System.Drawing.Size(24, 21);
            this.btnBlender.TabIndex = 10;
            this.btnBlender.Text = "...";
            this.btnBlender.UseVisualStyleBackColor = true;
            this.btnBlender.Click += new System.EventHandler(this.btnBlender_Click);
            // 
            // chbMergeFiles
            // 
            this.chbMergeFiles.AutoSize = true;
            this.chbMergeFiles.Location = new System.Drawing.Point(161, 78);
            this.chbMergeFiles.Name = "chbMergeFiles";
            this.chbMergeFiles.Size = new System.Drawing.Size(95, 17);
            this.chbMergeFiles.TabIndex = 29;
            this.chbMergeFiles.Text = "chbMergeFiles";
            this.chbMergeFiles.UseVisualStyleBackColor = true;
            this.chbMergeFiles.CheckedChanged += new System.EventHandler(this.chbMergeFiles_CheckedChanged);
            // 
            // lblExportAs
            // 
            this.lblExportAs.AutoSize = true;
            this.lblExportAs.Location = new System.Drawing.Point(6, 117);
            this.lblExportAs.Name = "lblExportAs";
            this.lblExportAs.Size = new System.Drawing.Size(59, 13);
            this.lblExportAs.TabIndex = 25;
            this.lblExportAs.Text = "lblExportAs";
            // 
            // cmbExportAs
            // 
            this.cmbExportAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExportAs.FormattingEnabled = true;
            this.cmbExportAs.Items.AddRange(new object[] {
            "BLEND",
            "DAE",
            "FBX",
            "OBJ"});
            this.cmbExportAs.Location = new System.Drawing.Point(9, 135);
            this.cmbExportAs.Name = "cmbExportAs";
            this.cmbExportAs.Size = new System.Drawing.Size(80, 21);
            this.cmbExportAs.TabIndex = 26;
            this.cmbExportAs.SelectedIndexChanged += new System.EventHandler(this.cmbExportAs_SelectedIndexChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(259, 179);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 23);
            this.btnExport.TabIndex = 32;
            this.btnExport.Text = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.Color.White;
            this.txtOutput.Enabled = false;
            this.txtOutput.Location = new System.Drawing.Point(9, 173);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(300, 20);
            this.txtOutput.TabIndex = 12;
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(315, 172);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(24, 21);
            this.btnOutput.TabIndex = 13;
            this.btnOutput.Text = "...";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(261, 71);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(78, 23);
            this.btnImport.TabIndex = 18;
            this.btnImport.Text = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // txtZSlang
            // 
            this.txtZSlang.BackColor = System.Drawing.Color.White;
            this.txtZSlang.Enabled = false;
            this.txtZSlang.Location = new System.Drawing.Point(9, 38);
            this.txtZSlang.Name = "txtZSlang";
            this.txtZSlang.Size = new System.Drawing.Size(300, 20);
            this.txtZSlang.TabIndex = 16;
            // 
            // btnZSlang
            // 
            this.btnZSlang.Location = new System.Drawing.Point(315, 37);
            this.btnZSlang.Name = "btnZSlang";
            this.btnZSlang.Size = new System.Drawing.Size(24, 21);
            this.btnZSlang.TabIndex = 17;
            this.btnZSlang.Text = "...";
            this.btnZSlang.UseVisualStyleBackColor = true;
            this.btnZSlang.Click += new System.EventHandler(this.btnZSlang_Click);
            // 
            // lblGothic1
            // 
            this.lblGothic1.AutoSize = true;
            this.lblGothic1.Location = new System.Drawing.Point(6, 22);
            this.lblGothic1.Name = "lblGothic1";
            this.lblGothic1.Size = new System.Drawing.Size(54, 13);
            this.lblGothic1.TabIndex = 2;
            this.lblGothic1.Text = "lblGothic1";
            // 
            // lblBlender
            // 
            this.lblBlender.AutoSize = true;
            this.lblBlender.Location = new System.Drawing.Point(6, 112);
            this.lblBlender.Name = "lblBlender";
            this.lblBlender.Size = new System.Drawing.Size(53, 13);
            this.lblBlender.TabIndex = 8;
            this.lblBlender.Text = "lblBlender";
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(6, 157);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(49, 13);
            this.lblOutput.TabIndex = 11;
            this.lblOutput.Text = "lblOutput";
            // 
            // txtGothic2
            // 
            this.txtGothic2.BackColor = System.Drawing.Color.White;
            this.txtGothic2.Enabled = false;
            this.txtGothic2.Location = new System.Drawing.Point(9, 83);
            this.txtGothic2.Name = "txtGothic2";
            this.txtGothic2.Size = new System.Drawing.Size(300, 20);
            this.txtGothic2.TabIndex = 6;
            // 
            // btnGothic2
            // 
            this.btnGothic2.Location = new System.Drawing.Point(315, 82);
            this.btnGothic2.Name = "btnGothic2";
            this.btnGothic2.Size = new System.Drawing.Size(24, 21);
            this.btnGothic2.TabIndex = 7;
            this.btnGothic2.Text = "...";
            this.btnGothic2.UseVisualStyleBackColor = true;
            this.btnGothic2.Click += new System.EventHandler(this.btnGothic2_Click);
            // 
            // lblGothic2
            // 
            this.lblGothic2.AutoSize = true;
            this.lblGothic2.Location = new System.Drawing.Point(6, 67);
            this.lblGothic2.Name = "lblGothic2";
            this.lblGothic2.Size = new System.Drawing.Size(54, 13);
            this.lblGothic2.TabIndex = 5;
            this.lblGothic2.Text = "lblGothic2";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(6, 165);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 27;
            this.lblVersion.Text = "lblVersion";
            // 
            // cmbVersion
            // 
            this.cmbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVersion.FormattingEnabled = true;
            this.cmbVersion.Items.AddRange(new object[] {
            "Gothic",
            "Gothic 2"});
            this.cmbVersion.Location = new System.Drawing.Point(9, 183);
            this.cmbVersion.Name = "cmbVersion";
            this.cmbVersion.Size = new System.Drawing.Size(80, 21);
            this.cmbVersion.TabIndex = 28;
            this.cmbVersion.SelectedIndexChanged += new System.EventHandler(this.cmbVersion_SelectedIndexChanged);
            // 
            // grpGeneral
            // 
            this.grpGeneral.Controls.Add(this.lblGothic1);
            this.grpGeneral.Controls.Add(this.btnGothic1);
            this.grpGeneral.Controls.Add(this.txtGothic1);
            this.grpGeneral.Controls.Add(this.txtGothic2);
            this.grpGeneral.Controls.Add(this.lblBlender);
            this.grpGeneral.Controls.Add(this.btnGothic2);
            this.grpGeneral.Controls.Add(this.txtBlender);
            this.grpGeneral.Controls.Add(this.lblGothic2);
            this.grpGeneral.Controls.Add(this.btnBlender);
            this.grpGeneral.Controls.Add(this.txtOutput);
            this.grpGeneral.Controls.Add(this.lblOutput);
            this.grpGeneral.Controls.Add(this.btnOutput);
            this.grpGeneral.Location = new System.Drawing.Point(12, 12);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(350, 205);
            this.grpGeneral.TabIndex = 1;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "grpGeneral";
            // 
            // lblZSlang
            // 
            this.lblZSlang.AutoSize = true;
            this.lblZSlang.Location = new System.Drawing.Point(6, 22);
            this.lblZSlang.Name = "lblZSlang";
            this.lblZSlang.Size = new System.Drawing.Size(51, 13);
            this.lblZSlang.TabIndex = 15;
            this.lblZSlang.Text = "lblZSlang";
            // 
            // grpImport
            // 
            this.grpImport.Controls.Add(this.lblZSlang);
            this.grpImport.Controls.Add(this.txtZSlang);
            this.grpImport.Controls.Add(this.btnImport);
            this.grpImport.Controls.Add(this.btnZSlang);
            this.grpImport.Location = new System.Drawing.Point(12, 223);
            this.grpImport.Name = "grpImport";
            this.grpImport.Size = new System.Drawing.Size(350, 100);
            this.grpImport.TabIndex = 14;
            this.grpImport.TabStop = false;
            this.grpImport.Text = "grpImport";
            // 
            // lblGothicSourcer
            // 
            this.lblGothicSourcer.AutoSize = true;
            this.lblGothicSourcer.Location = new System.Drawing.Point(6, 22);
            this.lblGothicSourcer.Name = "lblGothicSourcer";
            this.lblGothicSourcer.Size = new System.Drawing.Size(85, 13);
            this.lblGothicSourcer.TabIndex = 20;
            this.lblGothicSourcer.Text = "lblGothicSourcer";
            // 
            // grpExport
            // 
            this.grpExport.Controls.Add(this.cmbMergeObjects);
            this.grpExport.Controls.Add(this.lblMergeObjects);
            this.grpExport.Controls.Add(this.chbExportLevel);
            this.grpExport.Controls.Add(this.chbSkipInvisible);
            this.grpExport.Controls.Add(this.lblVersion);
            this.grpExport.Controls.Add(this.lblGothicSourcer);
            this.grpExport.Controls.Add(this.chbMergeFiles);
            this.grpExport.Controls.Add(this.cmbVersion);
            this.grpExport.Controls.Add(this.btnExport);
            this.grpExport.Controls.Add(this.txtGothicSourcer);
            this.grpExport.Controls.Add(this.cmbExportAs);
            this.grpExport.Controls.Add(this.lblExportAs);
            this.grpExport.Controls.Add(this.btnGothicSourcer);
            this.grpExport.Location = new System.Drawing.Point(12, 329);
            this.grpExport.Name = "grpExport";
            this.grpExport.Size = new System.Drawing.Size(350, 215);
            this.grpExport.TabIndex = 19;
            this.grpExport.TabStop = false;
            this.grpExport.Text = "grpExport";
            // 
            // cmbMergeObjects
            // 
            this.cmbMergeObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMergeObjects.FormattingEnabled = true;
            this.cmbMergeObjects.Location = new System.Drawing.Point(9, 87);
            this.cmbMergeObjects.Name = "cmbMergeObjects";
            this.cmbMergeObjects.Size = new System.Drawing.Size(80, 21);
            this.cmbMergeObjects.TabIndex = 24;
            this.cmbMergeObjects.SelectedIndexChanged += new System.EventHandler(this.cmbMergeObjects_SelectedIndexChanged);
            // 
            // lblMergeObjects
            // 
            this.lblMergeObjects.AutoSize = true;
            this.lblMergeObjects.Location = new System.Drawing.Point(6, 69);
            this.lblMergeObjects.Name = "lblMergeObjects";
            this.lblMergeObjects.Size = new System.Drawing.Size(83, 13);
            this.lblMergeObjects.TabIndex = 23;
            this.lblMergeObjects.Text = "lblMergeObjects";
            // 
            // chbExportLevel
            // 
            this.chbExportLevel.AutoSize = true;
            this.chbExportLevel.Location = new System.Drawing.Point(161, 124);
            this.chbExportLevel.Name = "chbExportLevel";
            this.chbExportLevel.Size = new System.Drawing.Size(100, 17);
            this.chbExportLevel.TabIndex = 31;
            this.chbExportLevel.Text = "chbExportLevel";
            this.chbExportLevel.UseVisualStyleBackColor = true;
            this.chbExportLevel.CheckedChanged += new System.EventHandler(this.chbExportLevel_CheckedChanged);
            // 
            // chbSkipInvisible
            // 
            this.chbSkipInvisible.AutoSize = true;
            this.chbSkipInvisible.Location = new System.Drawing.Point(161, 101);
            this.chbSkipInvisible.Name = "chbSkipInvisible";
            this.chbSkipInvisible.Size = new System.Drawing.Size(103, 17);
            this.chbSkipInvisible.TabIndex = 30;
            this.chbSkipInvisible.Text = "chbSkipInvisible";
            this.chbSkipInvisible.UseVisualStyleBackColor = true;
            this.chbSkipInvisible.CheckedChanged += new System.EventHandler(this.chbSkipInvisible_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 556);
            this.Controls.Add(this.grpExport);
            this.Controls.Add(this.grpImport);
            this.Controls.Add(this.grpGeneral);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "ZenVis";
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpImport.ResumeLayout(false);
            this.grpImport.PerformLayout();
            this.grpExport.ResumeLayout(false);
            this.grpExport.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}