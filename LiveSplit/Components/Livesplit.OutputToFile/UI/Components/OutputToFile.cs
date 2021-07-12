using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class OutputToFile : IComponent
    {
        public OutputToFileSettings Settings { get; set; }
        public float VerticalHeight { get; set; }
        public GraphicsCache Cache { get; set; }

        public float MinimumWidth => 0f;

        public float HorizontalWidth
        {
            get
            {
                return 0f;
            }
        }

        public IDictionary<string, Action> ContextMenuControls => null;

        public float PaddingTop => 0f;
        public float PaddingLeft => 0f;
        public float PaddingBottom => 0f;
        public float PaddingRight => 0f;

        public float MinimumHeight { get; set; }

        public OutputToFile()
        {
            VerticalHeight = 0;
            Settings = new OutputToFileSettings();
            Cache = new GraphicsCache();
            MakeDirectory();
        }

        public string ComponentName => "Output to File";

        public Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // attempt history count
            Cache.Restart();
            Cache["AttemptHistoryCount"] = state.Run.AttemptHistory.Count;
            Cache["Run"] = state.Run;
            if (Cache.HasChanged) {
                MakeFile("AttemptCount.txt", Cache["AttemptHistoryCount"].ToString());
            }

            // run & category information
            Cache.Restart();
            Cache["GameName"] = state.Run.GameName;
            Cache["CategoryName"] = state.Run.CategoryName;
            if (Cache.HasChanged) {
                MakeFile("GameName.txt", Cache["GameName"].ToString());
                MakeFile("CategoryName.txt", Cache["CategoryName"].ToString());
            }

            // split (real time)
            Cache.Restart();
            var currentSplit = state.CurrentSplit;
            if (currentSplit == null) {
                Cache["CurrentSplitName"] = null;
                Cache["CurrentSplitGoldTime"] = null;
                Cache["CurrentSplitPBTime"] = null;
                if (Cache.HasChanged) {
                    MakeFile("CurrentSplit_Name.txt", "<none>");
                    MakeFile("CurrentSplit_RealTime_Gold.txt", "N/A");
                    MakeFile("CurrentSplit_RealTime_PB.txt", "N/A");
                }
            }
            else {
                Cache["CurrentSplitName"] = currentSplit.Name;
                Cache["CurrentSplitGoldTime"] = currentSplit.BestSegmentTime[TimingMethod.RealTime];
                Cache["CurrentSplitPBTime"] = currentSplit.PersonalBestSplitTime[TimingMethod.RealTime];
                if (Cache.HasChanged) {
                    MakeFile("CurrentSplit_Name.txt", Cache["CurrentSplitName"].ToString());
                    if (Cache["CurrentSplitGoldTime"] == null) 
                        MakeFile("CurrentSplit_RealTime_Gold.txt", "N/A");
                    else 
                        MakeFile("CurrentSplit_RealTime_Gold.txt", Cache["CurrentSplitGoldTime"].ToString());

                    if (Cache["CurrentSplitPBTime"] == null)
                        MakeFile("CurrentSplit_RealTime_PB.txt", "N/A");
                    else
                        MakeFile("CurrentSplit_RealTime_PB.txt", Cache["CurrentSplitPBTime"].ToString());
                }
            }
        }

        public void Dispose() { }
        
        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();

        void MakeDirectory() {
            string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LiveSplitTEST");
            if (!Directory.Exists(settingsPath)) {
                Directory.CreateDirectory(settingsPath);
            }
        }

        void MakeFile(string fileName, string items) {
            string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LiveSplitTEST"); 
            string path = Path.Combine(settingsPath, fileName);
            File.WriteAllText(path, items);
        }
    }
}
