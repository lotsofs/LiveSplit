using LiveSplit.TimeFormatters;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components {
    public partial class OutputToFileSettings : UserControl {
        public string ScriptPath { get; set; }

        public OutputToFileSettings() {
            InitializeComponent();

            ScriptPath = string.Empty;

            //this.txtScriptPath.DataBindings.Add("Text", this, "ScriptPath", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void TimerSettings_Load(object sender, EventArgs e) {

        }

        public void SetSettings(XmlNode node) {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            ScriptPath = SettingsHelper.ParseString(element["ScriptPath"], string.Empty);
        }

        public XmlNode GetSettings(XmlDocument document) {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode() {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent) {
            return SettingsHelper.CreateSetting(document, parent, "Version", "0.1") ^
            SettingsHelper.CreateSetting(document, parent, "ScriptPath", ScriptPath);
        }

		private void InitializeComponent() {
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.labelScriptPath = new System.Windows.Forms.Label();
			this.btnSelectFile = new System.Windows.Forms.Button();
			this.txtScriptPath = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.labelScriptPath, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnSelectFile, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.txtScriptPath, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(415, 151);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// labelScriptPath
			// 
			this.labelScriptPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelScriptPath.AutoSize = true;
			this.labelScriptPath.Location = new System.Drawing.Point(3, 8);
			this.labelScriptPath.Name = "labelScriptPath";
			this.labelScriptPath.Size = new System.Drawing.Size(70, 13);
			this.labelScriptPath.TabIndex = 3;
			this.labelScriptPath.Text = "Script Path:";
			// 
			// btnSelectFile
			// 
			this.btnSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectFile.Location = new System.Drawing.Point(338, 3);
			this.btnSelectFile.Name = "btnSelectFile";
			this.btnSelectFile.Size = new System.Drawing.Size(74, 23);
			this.btnSelectFile.TabIndex = 1;
			this.btnSelectFile.Text = "Browse...";
			this.btnSelectFile.UseVisualStyleBackColor = true;
			// 
			// txtScriptPath
			// 
			this.txtScriptPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtScriptPath.Location = new System.Drawing.Point(79, 4);
			this.txtScriptPath.Name = "txtScriptPath";
			this.txtScriptPath.Size = new System.Drawing.Size(253, 20);
			this.txtScriptPath.TabIndex = 0;
			// 
			// OutputToFileSettings
			// 
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "OutputToFileSettings";
			this.Size = new System.Drawing.Size(415, 151);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
