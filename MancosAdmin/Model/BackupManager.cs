using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MancosAdmin.Model
{
    class BackupManager : INotifyPropertyChanged
    {
        public List<string> BackupList;
        public string BackupPath = MainWindow.ServerPath + "\\ServerBackups";
        public string WorldPath = MainWindow.ServerPath + "\\world";
        private int hoursToNextBackup;
        private int minutesToNextBackup;
        private string lastBackup;
        private TimeSpan nextBackupIn;

        public int HoursToNextBackup
        {
            get
            {
                return hoursToNextBackup;
            }
            set
            {
                hoursToNextBackup = value;
                NotifyPropertyChanged("HoursToNextBackup");
            }
        }
        public int MinutesToNextBackup
        {
            get
            {
                return minutesToNextBackup;
            }
            set
            {
                minutesToNextBackup = value;
                NotifyPropertyChanged("HoursToNextBackup");
            }
        }
        public string LastBackup
        {
            get
            {
                return lastBackup;
            }
            set
            {
                lastBackup = value;
                NotifyPropertyChanged("LastBackup");
            }
        }
        public TimeSpan NextBackupIn
        {
            get
            {
                return nextBackupIn;
            }
            set
            {
                nextBackupIn = value;
                NotifyPropertyChanged("NextBackupIn");
            }
        }
    

        public BackupManager()
        {
            if (!Directory.Exists(BackupPath))
            {
                Directory.CreateDirectory(BackupPath);
            }
            updateBackupList();
        }

        private void updateBackupList()
        {
            BackupList = Directory.EnumerateDirectories(BackupPath).ToList();
        }

        public void deleteOldBackups()
        {
            if (BackupList.Count > 9)
            {
                DateTime oldest = DateTime.Now;
                string oldestFilename = "";
                string temp = "";
                string[] temparray;
                foreach (string filename in BackupList)
                {
                    temparray = filename.Split('\\');
                    temp = temparray[temparray.Length - 1];
                    temp = temp.Replace(".", "/");
                    temp = temp.Replace("-", " ");
                    temp = temp.Replace(",", ":");
                    if (DateTime.Parse(temp).CompareTo(oldest) == -1)
                    {
                        oldest = DateTime.Parse(temp);
                        oldestFilename = filename;
                    }
                }
                Directory.Delete(oldestFilename, true);
            }
            updateBackupList();
        }

        public void createBackup()
        {
            string dateTime = DateTime.Now.ToString().Replace("/", ".");
            dateTime = dateTime.Replace(" ", "-");
            dateTime = dateTime.Replace(":", ",");
            string newfolderbackuppath = BackupPath + "\\" + dateTime;
            DirectoryCopy(WorldPath, newfolderbackuppath, true);
            deleteOldBackups();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }


    }
}
