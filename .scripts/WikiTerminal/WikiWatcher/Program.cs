using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WikiWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Context());
        }
    }

    class Context : ApplicationContext
    {
        private Settings settings;
        private NotifyIcon icon;
        private Timer timer;
        private List<string> logCache;
        private StreamWriter logStream;
        private Dictionary<string, WatchResult> pendingResults;

        public Context()
        {
            if (!Debugger.IsAttached)
                Environment.CurrentDirectory = Path.Combine(Environment.GetCommandLineArgs()[0], "..");

            #region Initialise objects
            string settings_path = "WikiWatcher.json";
            if (File.Exists(settings_path))
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settings_path));
            else
                settings = new Settings();

            icon = new NotifyIcon()
            {
                Text = "WikiWatcher",
                Icon = new System.Drawing.Icon("logo.ico"),
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Resolve pending conflicts", ResolveConflicts),
                    new MenuItem("Scan for changes", ScanPages),
                    new MenuItem("Open log", OpenLog),
                    new MenuItem("Settings", OpenSettings),
                    new MenuItem("Quit", Quit)
                }),
                Visible = true
            };
            icon.MouseClick += Icon_MouseClick;
            icon.BalloonTipClicked += Icon_BalloonTipClicked;
            
            timer = new Timer()
            {
                Enabled = false
            };
            timer.Tick += ScanPages;

            pendingResults = new Dictionary<string, WatchResult>();
            #endregion

            #region Get log
            string log_path = "Watcher.log";
            if (File.Exists(log_path))
                logCache = File.ReadAllLines(log_path, Encoding.UTF8).ToList();
            else
            {
                File.Create(log_path);
                logCache = new List<string>();
            }
            while (logStream == null)
            {
                try
                {
                    logStream = new StreamWriter(log_path);
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            #endregion
            foreach (string entry in logCache)
                logStream.WriteLine(entry);

            timer.Interval = 1;
            timer.Start();
        }

        private void Icon_BalloonTipClicked(object sender, EventArgs e)
        {
            ResolveConflicts(null, null);
        }

        private void ScanPages(object sender, EventArgs e)
        {
            NewLogEntry("Checking pages for updates");
            timer.Stop();
            string[] pagedatas = GetFilesRecursive("pagedata");
            bool user_confirm_needed = false;
            foreach (string pagedata_file in pagedatas)
            {
                if (pendingResults.ContainsKey(pagedata_file))
                    continue;

                Process proc = new Process()
                {
                    StartInfo = new ProcessStartInfo("python", $"watch.py \"{pagedata_file.Substring(9, pagedata_file.Length - 14)}\"")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = !Debugger.IsAttached
                    }
                };
                proc.Start();
                proc.WaitForExit();

                WatchResult result = JsonConvert.DeserializeObject<WatchResult>(File.ReadAllText("WatchResult.json", Encoding.UTF8));
                switch (result.result)
                {
                    case 0:
                        NewLogEntry($"{pagedata_file}: No changes");
                        break;
                    case 1:
                        NewLogEntry($"{pagedata_file}: Successful change");
                        break;
                    case 2:
                        NewLogEntry($"{pagedata_file}: Change detected. User confirmation required");
                        pendingResults.Add(pagedata_file, result);
                        user_confirm_needed = true;
                        break;
                }
            }
            if (user_confirm_needed)
                icon.ShowBalloonTip(10000, "Wiki page conflict", "There's a conflict between the local and most recent version of a page on your system. Click here to resolve it.", ToolTipIcon.Info);
            if (settings.keepOpenWithoutVSCode || Process.GetProcessesByName("Code").Length > 0)
            {
                NewLogEntry($"Check complete. Next check in {settings.scanInterval} seconds.");
                timer.Interval = settings.scanInterval * 1000;
                timer.Start();
            }
            else
            {
                NewLogEntry("Check complete. Shutting down.");
                Quit(null, null);
            }
        }

        private string[] GetFilesRecursive(string path)
        {
            List<string> files = Directory.GetFiles(path).ToList();
            foreach (string dir in Directory.GetDirectories(path))
                files.AddRange(GetFilesRecursive(dir));
            return files.ToArray();
        }

        private void Icon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ResolveConflicts(null, null);
        }

        private void NewLogEntry(string entry)
        {
            string log_entry = $"{DateTime.Now}: {entry}";
            Console.WriteLine(log_entry);
            logCache.Add(log_entry);
            logStream.WriteLine(log_entry);
            logStream.Flush();
        }

        private void ResolveConflicts(object sender, EventArgs e)
        {
            new ConflictsForm(pendingResults).ShowDialog();
            foreach (string pagedata_path in pendingResults.Keys.ToArray())
            {
                WatchResult result = pendingResults[pagedata_path];
                if (result.resolve_action > 0)
                {
                    if (result.resolve_action == 1)
                    {
                        NewLogEntry($"{pagedata_path}: User resolved by overwriting local");
                        string page_path = Path.Combine("..", pagedata_path.Substring(9, pagedata_path.Length - 14));
                        File.WriteAllText(page_path, result.content);
                    }
                    else
                        NewLogEntry($"{pagedata_path}: User resolved by ignoring new version");
                    Pagedata pagedata = JsonConvert.DeserializeObject<Pagedata>(File.ReadAllText(pagedata_path, Encoding.UTF8));
                    pagedata.version = result.new_version;
                    pagedata.version_hash = result.new_hash;
                    File.WriteAllText(pagedata_path, JsonConvert.SerializeObject(pagedata), Encoding.UTF8);
                    pendingResults.Remove(pagedata_path);
                }
            }
        }

        private void OpenLog(object sender, EventArgs e)
        {
            new LogForm(logCache).Show();
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            SettingsForm sForm = new SettingsForm(settings);
            sForm.ShowDialog();
            if (sForm.dialogResult == DialogResult.OK)
                File.WriteAllText("WikiWatcher.json", JsonConvert.SerializeObject(settings));
        }

        private void Quit(object sender, EventArgs e)
        {
            icon.Visible = false;
            logStream.Close();
            Application.Exit();
        }
    }
}
