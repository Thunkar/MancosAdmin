using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MancosAdmin.Model
{
    class ServerWrapper : INotifyPropertyChanged
    {
        public Process ServerProc;
        private string minRam;
        private string maxRam;
        public string MinRAM
        {
            get
            {
                return minRam;
            }
            set
            {
                minRam = value;
                NotifyPropertyChanged("MinRAM");
            }
        }
        public string MaxRAM
        {
            get
            {
                return maxRam;
            }
            set
            {
                maxRam = value;
                NotifyPropertyChanged("MaxRAM");
            }
        }
        private string ram;
        public string RAM
        {
            get
            {
                return ram;
            }
            set
            {
                ram = value;
                NotifyPropertyChanged("RAM");
            }
        }
        private bool stopped;
        public bool Stopped
        {
            get
            {
                return stopped;
            }
            set
            {
                stopped = value;
                NotifyPropertyChanged("Running");
            }
        }
        public ObservableSet<Player> Players;
        public ObservableCollection<ConsoleInput> consoleData;
        public DispatcherTimer ramtimer;


        public ServerWrapper()
        {
            Players = new ObservableSet<Player>();
            consoleData = new ObservableCollection<ConsoleInput>();
            TimeSpan ramspan = new TimeSpan(0, 0, 0, 1);
            ramtimer = new DispatcherTimer();
            ramtimer.Interval = ramspan;
            ramtimer.Tick += ramtimer_Tick;
            Stopped = true;
        }

        public void serverInitialize()
        {
            ServerProc = new Process();
            string starter = "-Xms" + MinRAM + " -Xmx" + MaxRAM + " -jar " + MainWindow.ServerFile + " nogui -d64";
            var startInfo = new ProcessStartInfo(MainWindow.JavaPath + "\\" + MainWindow.JavaFile, starter);
            startInfo.WorkingDirectory = MainWindow.ServerPath;
            startInfo.RedirectStandardInput = startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            ServerProc.StartInfo = startInfo;
            ServerProc.EnableRaisingEvents = true;
        }

        void ramtimer_Tick(object sender, EventArgs e)
        {
            RAM = ((Process.GetProcessesByName("java")[0].WorkingSet64 / 1024) / 1024).ToString() + " MB";
        }

        public void startServer()
        {
            ServerProc.Start();
            Stopped = false;
            ServerProc.BeginErrorReadLine();
            ramtimer.Start();
        }

        public void stopServer()
        {
            try
            {
                ServerProc.StandardInput.WriteLine("stop");
                ServerProc.WaitForExit(10000);
                ServerProc.CancelErrorRead();
                ramtimer.Stop();
                RAM = "";
                Players.Clear();
                Stopped = true;
            }
            catch (Exception)
            {

            }
        }

        public void ParseServerInput(string input)
        {
            if (input != null && input.Contains("User") && input.Contains("connecting"))
            {
                int nameEnd = input.IndexOf(" connecting");
                string name = input.Remove(nameEnd);
                int nameStart = input.IndexOf("User ");
                name = name.Substring(nameStart);
                name = name.Replace("User ", "");
                name = name.Trim();
                Player tobeadded = new Player(name);
                Players.Add(tobeadded);
            }
            if (input != null && input.Contains("lost connection"))
            {
                try
                {
                    foreach (Player player in Players)
                    {
                        if (input.Contains(player.Name)) Players.Remove(player);
                    }
                }
                catch (Exception) { }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
