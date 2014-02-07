
using MancosAdmin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MancosAdmin.ViewModel
{
    class MainViewModel
    {
        public static MainViewModel Current { get; set; }
        public ServerWrapper ServerWrapper { get; set; }
        public BackupManager BackupManager { get; set; }


        public MainViewModel()
        {
            Current = this;
            ServerWrapper = new ServerWrapper();
            BackupManager = new BackupManager();
        }
    }
}
