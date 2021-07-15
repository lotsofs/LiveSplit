using LiveSplit.Model;
using LiveSplit.TimeFormatters;
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

        protected ShortTimeFormatter Formatter = new ShortTimeFormatter();

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
            Cache["AttemptHistoryCount"] = state.Run.AttemptCount;
            Cache["Run"] = state.Run;
            // run & category information
            Cache["GameName"] = state.Run.GameName;
            Cache["CategoryName"] = state.Run.CategoryName;
            Cache["State"] = state.CurrentPhase;
            if (Cache.HasChanged) {
                MakeFile("GameName.txt", Cache["GameName"].ToString());
                MakeFile("CategoryName.txt", Cache["CategoryName"].ToString());
                MakeFile("AttemptCount.txt", Cache["AttemptHistoryCount"].ToString());
            }

            // split (real time)
            Cache.Restart();
            var currentSplit = state.CurrentSplit;
            if (state.CurrentPhase == TimerPhase.NotRunning) {
                Cache["CurrentSplitName"] = null;
                Cache["CurrentSplitIndex"] = -1;
                Cache["CurrentSplitGoldTime"] = null;
                Cache["CurrentSplitPBTime"] = null;
                Cache["PreviousSplitSign"] = "Undetermined";
                if (Cache.HasChanged) {
                    MakeFile("CurrentSplit_Name.txt", "-");
                    MakeFile("CurrentSplit_RealTime_Gold.txt", "-");
                    MakeFile("CurrentSplit_RealTime_PB.txt", "-");
                    MakeFile("CurrentSplit_Index.txt", "-1");
                    MakeFile("PreviousSplit_Sign.txt", "Undetermined");
                }
            }
            else if (state.CurrentPhase == TimerPhase.Ended) {
                Cache["CurrentSplitName"] = null;
                Cache["CurrentSplitIndex"] = state.Run.Count;
                Cache["CurrentSplitGoldTime"] = null;
                Cache["CurrentSplitPBTime"] = null;

                TimeSpan? previousSplitTime = state.Run[state.CurrentSplitIndex - 1].SplitTime[TimingMethod.RealTime];
                TimeSpan? previousSplitPBTime = state.Run[state.CurrentSplitIndex - 1].PersonalBestSplitTime[TimingMethod.RealTime];
                Cache["PreviousSplitSign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "NoPB" : "PB";

                if (Cache.HasChanged) {
                    MakeFile("CurrentSplit_Name.txt", "-");
                    MakeFile("CurrentSplit_RealTime_Gold.txt", "-");
                    MakeFile("CurrentSplit_RealTime_PB.txt", "-");
                    MakeFile("CurrentSplit_Index.txt", Cache["CurrentSplitIndex"].ToString());
                    MakeFile("PreviousSplit_Sign.txt", Cache["PreviousSplitSign"].ToString());
                }
            }
            else {
                Cache["CurrentSplitName"] = currentSplit.Name;
                Cache["CurrentSplitIndex"] = state.CurrentSplitIndex;
                Cache["CurrentSplitGoldTime"] = currentSplit.BestSegmentTime[TimingMethod.RealTime];


                // calculate the PB split as this value tracks the full run time instead of individual split time
                if (state.CurrentSplitIndex > 0) {
                    var previousSplit = state.Run[state.CurrentSplitIndex - 1];
                    Cache["CurrentSplitPBTime"] = currentSplit.PersonalBestSplitTime[TimingMethod.RealTime] - previousSplit.PersonalBestSplitTime[TimingMethod.RealTime];
                    // calculate whether the run is ahead or behind
                    TimeSpan? previousSplitTime = previousSplit.SplitTime[TimingMethod.RealTime];
                    TimeSpan? previousSplitPBTime = previousSplit.PersonalBestSplitTime[TimingMethod.RealTime];
                    if (previousSplitTime != null && previousSplitPBTime != null) {
                        Cache["PreviousSplitSign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "Behind" : "Ahead";
                    }
                    else {
                        Cache["PreviousSplitSign"] = "Undetermined";
				    }
                }
                else {
                    Cache["CurrentSplitPBTime"] = currentSplit.PersonalBestSplitTime[TimingMethod.RealTime];
                    Cache["PreviousSplitSign"] = "Undetermined";
                }


                if (Cache.HasChanged) {
                    MakeFile("CurrentSplit_Name.txt",  Cache["CurrentSplitName"].ToString());
                    MakeFile("CurrentSplit_Index.txt", Cache["CurrentSplitIndex"].ToString());
                    MakeFile("PreviousSplit_Sign.txt", Cache["PreviousSplitSign"].ToString());

                    // write the gold split
                    if (Cache["CurrentSplitGoldTime"] == null)
                        MakeFile("CurrentSplit_RealTime_Gold.txt", "-");
                    else {
                        var timeString = Formatter.Format((TimeSpan)Cache["CurrentSplitGoldTime"], TimeFormat.Minutes);
                        int dotIndex = timeString.IndexOf(".");
                        MakeFile("CurrentSplit_RealTime_Gold.txt", timeString.Substring(0, dotIndex + 3).ToString());
                    }

                    // write the PB split
                    if (Cache["CurrentSplitPBTime"] == null)
                        MakeFile("CurrentSplit_RealTime_PB.txt", "-");
                    else {
                        var timeString = Formatter.Format((TimeSpan)Cache["CurrentSplitPBTime"], TimeFormat.Minutes);
                        int dotIndex = timeString.IndexOf(".");
                        MakeFile("CurrentSplit_RealTime_PB.txt", timeString.Substring(0, dotIndex + 3).ToString());
                    }
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
            string settingsPath = Settings.FolderPath;
            if (string.IsNullOrEmpty(settingsPath)) return;
            string path = Path.Combine(settingsPath, fileName);
            File.WriteAllText(path, items);
        }
    }
}
