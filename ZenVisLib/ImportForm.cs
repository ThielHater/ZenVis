using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ZenVis.Shared;

namespace ZenVis.Lib
{
    public class ImportForm : Form
    {
        public Dictionary<string, ImportSetting> Settings;

        private Dictionary<string, Dictionary<string, string>> TypesProperties;

        private List<PropertyData> CurrentProperties;

        private IContainer components;

        private Label lblType;

        private ComboBox cmbType;

        private Label lblProperties;

        private ListBox lstVisuals;

        private DataGridView gridProperties;

        private Button btnClose;

        public ImportForm(Dictionary<string, ImportSetting> settings, Dictionary<string, Dictionary<string, string>> typesProperties, List<Visual> visuals)
        {
            this.InitializeComponent();
            this.Settings = settings;
            this.TypesProperties = typesProperties;
            this.CurrentProperties = new List<PropertyData>();
            this.lblType.Text = Localization.Instance.GetTranslation("lblType", null);
            this.lblProperties.Text = Localization.Instance.GetTranslation("lblProperties", null);
            this.btnClose.Text = Localization.Instance.GetTranslation("btnClose", null);
            SortedSet<string> strs = new SortedSet<string>();
            foreach (Visual visual in visuals)
            {
                if (!visual.FileName.StartsWith("LEVEL"))
                {
                    strs.Add(visual.FileName);
                }
            }
            foreach (string str in strs)
            {
                this.lstVisuals.Items.Add(str);
            }
            if (this.lstVisuals.Items.Count > 0)
            {
                this.lstVisuals.SelectedIndex = 0;
            }
        }

        private void lstVisuals_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = (string)this.lstVisuals.SelectedItem;

            this.gridProperties.DataSource = null;
            this.cmbType.Items.Clear();

            foreach (KeyValuePair<string, Dictionary<string, string>> typesProperty in this.TypesProperties)
            {
                this.cmbType.Items.Add(typesProperty.Key);
            }
            this.cmbType.Items.Add("[...]");

            HashSet<string> types = new HashSet<string>();

            foreach (string item in this.lstVisuals.SelectedItems)
            {
                if (this.Settings.ContainsKey(item))
                {
                    types.Add(this.Settings[item].Type);
                }
                else
                {
                    types.Add("");
                }
            }

            if (types.Count == 0)
            {
                this.cmbType.SelectedItem = "";
            }
            else if (types.Count == 1)
            {
                this.cmbType.SelectedItem = types.First();
            }
            else
            {
                this.cmbType.SelectedItem = "[...]";
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = (string)this.cmbType.SelectedItem;

            if (type == "[...]")
            {
                return;
            }

            this.gridProperties.DataSource = null;
            this.CurrentProperties.Clear();

            foreach (string item in this.lstVisuals.SelectedItems)
            {
                if (!this.Settings.ContainsKey(item))
                {
                    this.Settings.Add(item, new ImportSetting());
                }
                else
                {
                    foreach (KeyValuePair<string, string> prop in this.Settings[item].Properties.ToList())
                    {
                        if (!this.TypesProperties[type].ContainsKey(prop.Key))
                        {
                            this.Settings[item].Properties.Remove(prop.Key);
                        }
                    }
                }
                this.Settings[item].Type = type;
            }

            foreach (KeyValuePair<string, string> prop in this.TypesProperties[type])
            {
                SortedSet<string> values = new SortedSet<string>();

                foreach (string item in this.lstVisuals.SelectedItems)
                {
                    if (this.Settings[item].Properties.ContainsKey(prop.Key))
                    {
                        values.Add(this.Settings[item].Properties[prop.Key]);
                    }
                    else
                    {
                        values.Add("");
                    }
                }

                if (values.Count == 0)
                {
                    this.CurrentProperties.Add(new PropertyData(prop.Key, prop.Value, ""));
                }
                else if (values.Count == 1)
                {
                    this.CurrentProperties.Add(new PropertyData(prop.Key, prop.Value, values.First()));
                }
                else
                {
                    this.CurrentProperties.Add(new PropertyData(prop.Key, prop.Value, "[...]"));
                }
            }

            this.gridProperties.DataSource = this.CurrentProperties;
            this.gridProperties.Columns[0].ReadOnly = true;
            this.gridProperties.Columns[1].ReadOnly = true;
            this.gridProperties.Rows[0].Cells[2].Selected = true;
        }

        private void gridProperties_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string name = (string)this.gridProperties.Rows[e.RowIndex].Cells[0].Value;
            string type = (string)this.gridProperties.Rows[e.RowIndex].Cells[1].Value;
            string data = (string)this.gridProperties.Rows[e.RowIndex].Cells[2].Value;

            if (data == "[...]")
            {
                return;
            }

            if (!ZenVisImp.CheckVariableValue(type, data))
            {
                MessageBox.Show(Localization.Instance.GetTranslation("TypeValueError", null), "ZenVis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.gridProperties.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                return;
            }

            foreach (string item in this.lstVisuals.SelectedItems)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    if (!this.Settings.ContainsKey(item))
                    {
                        this.Settings.Add(item, new ImportSetting());
                    }
                    if (!this.Settings[item].Properties.ContainsKey(name))
                    {
                        this.Settings[item].Properties.Add(name, data);
                    }
                    this.Settings[item].Properties[name] = data;
                }
                else
                {
                    if (this.Settings.ContainsKey(item) && this.Settings[item].Properties.ContainsKey(name))
                    {
                        this.Settings[item].Properties.Remove(name);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblProperties = new System.Windows.Forms.Label();
            this.lstVisuals = new System.Windows.Forms.ListBox();
            this.gridProperties = new System.Windows.Forms.DataGridView();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(234, 12);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(41, 13);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "lblType";
            // 
            // cmbType
            // 
            this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbType.BackColor = System.Drawing.Color.White;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(237, 28);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(180, 21);
            this.cmbType.TabIndex = 3;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // lblProperties
            // 
            this.lblProperties.AutoSize = true;
            this.lblProperties.Location = new System.Drawing.Point(234, 61);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(64, 13);
            this.lblProperties.TabIndex = 4;
            this.lblProperties.Text = "lblProperties";
            // 
            // lstVisuals
            // 
            this.lstVisuals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstVisuals.BackColor = System.Drawing.Color.White;
            this.lstVisuals.FormattingEnabled = true;
            this.lstVisuals.Location = new System.Drawing.Point(12, 12);
            this.lstVisuals.Name = "lstVisuals";
            this.lstVisuals.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstVisuals.Size = new System.Drawing.Size(200, 420);
            this.lstVisuals.TabIndex = 1;
            this.lstVisuals.SelectedIndexChanged += new System.EventHandler(this.lstVisuals_SelectedIndexChanged);
            // 
            // gridProperties
            // 
            this.gridProperties.AllowUserToAddRows = false;
            this.gridProperties.AllowUserToDeleteRows = false;
            this.gridProperties.AllowUserToResizeRows = false;
            this.gridProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridProperties.BackgroundColor = System.Drawing.Color.White;
            this.gridProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProperties.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gridProperties.Location = new System.Drawing.Point(237, 77);
            this.gridProperties.MultiSelect = false;
            this.gridProperties.Name = "gridProperties";
            this.gridProperties.RowHeadersVisible = false;
            this.gridProperties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridProperties.ShowCellErrors = false;
            this.gridProperties.ShowCellToolTips = false;
            this.gridProperties.ShowRowErrors = false;
            this.gridProperties.Size = new System.Drawing.Size(415, 354);
            this.gridProperties.TabIndex = 5;
            this.gridProperties.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridProperties_CellEndEdit);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(577, 446);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 482);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridProperties);
            this.Controls.Add(this.lstVisuals);
            this.Controls.Add(this.lblProperties);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "ImportForm";
            this.Text = "ZenVis";
            ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}