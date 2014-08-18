
using MancosAdmin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MancosAdmin.ViewModel
{
    class MainViewModel
    {
        public static MainViewModel Current { get; set; }
        public ServerWrapper ServerWrapper { get; set; }
        public BackupManager BackupManager { get; set; }
        public Timer backupTimer {get; set;}
        public Timer countDownTimer { get; set; }



        public MainViewModel()
        {
            Current = this;
            ServerWrapper = new ServerWrapper();
            BackupManager = new BackupManager();
            backupTimer = new Timer();
            countDownTimer = new Timer();

            TimeSpan countDownSpan = new TimeSpan(0, 0, 0, 60);
            countDownTimer.Interval = countDownSpan.TotalMilliseconds;
            countDownTimer.Elapsed += countDownTimer_Elapsed;
            backupTimer.Elapsed += backupTimer_Elapsed;
        }

        void backupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServerWrapper.ServerProc.StandardInput.WriteLine("/say Time for a backup, bitches! You have 1 minute left.");
            backupTimer.Stop();
            countDownTimer.Start();
        }

        void countDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MainWindow.GUINextBackup.Stop();
            BackupManager.NextBackupIn = new TimeSpan(0, BackupManager.HoursToNextBackup, BackupManager.MinutesToNextBackup, 0);
            BackupManager.LastBackup = "Last backup: " + DateTime.Now.ToString();
            TimeSpan backupSpan = new TimeSpan(0, BackupManager.HoursToNextBackup, BackupManager.MinutesToNextBackup - 1, 0);
            backupTimer.Interval = (double)(backupSpan.TotalMilliseconds);
            ServerWrapper.stopServer();
            BackupManager.createBackup();
            ServerWrapper.startServer();
            countDownTimer.Stop();
            MainWindow.GUINextBackup.Start();
            backupTimer.Start();
        }


        public void LaunchServer()
        {
            BackupManager.NextBackupIn = new TimeSpan(0, BackupManager.HoursToNextBackup, BackupManager.MinutesToNextBackup, 0);
            TimeSpan backupSpan = new TimeSpan(0, BackupManager.HoursToNextBackup, BackupManager.MinutesToNextBackup-1, 0);
            backupTimer.Interval = (double)(backupSpan.TotalMilliseconds);
            ServerWrapper.serverInitialize();
            ServerWrapper.startServer();
            MainWindow.GUINextBackup.Start();
            backupTimer.Start();
        }

        internal void StopServer()
        {
            ServerWrapper.stopServer();
            backupTimer.Stop();
        }
    }
}
