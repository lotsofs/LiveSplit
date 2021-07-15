﻿using LiveSplit.TimeFormatters;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components {
    public partial class OutputToFileSettings : UserControl {
        public string FolderPath { get; set; }

        public OutputToFileSettings() {
            InitializeComponent();

            FolderPath = string.Empty;

            this.txtFolderPath.DataBindings.Add("Text", this, "FolderPath", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void TimerSettings_Load(object sender, EventArgs e) {

        }

        public void SetSettings(XmlNode node) {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            FolderPath = SettingsHelper.ParseString(element["FolderPath"], string.Empty);
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
            SettingsHelper.CreateSetting(document, parent, "FolderPath", FolderPath);
        }

        private void btnSelectFile_Click(object sender, EventArgs e) {
            var dialog = new FolderBrowserDialog();

            if (Directory.Exists(FolderPath)) {
                dialog.SelectedPath = Path.GetDirectoryName(FolderPath);
            }

            if (dialog.ShowDialog() == DialogResult.OK) {
                FolderPath = this.txtFolderPath.Text = dialog.SelectedPath;
            }
        }
	}
}
