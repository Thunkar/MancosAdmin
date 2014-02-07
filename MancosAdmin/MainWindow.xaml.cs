
using MancosAdmin.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace MancosAdmin
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string ConsumerKey = "9iX4YycdjFw02Zy1MnBQ";
        public string ConsumerSecret = "M9tlAedbQREWNs7pwspJyHri9UnqqbzFrHDK0Tq8";
        public string AccessToken = "471393678-44lDVnvJdSsmsCRfRcI6urlGlRdLX9KzwG9PavzN";
        public string AccessTokenSecret = "iQKXTNKJzLhNXvFrFc7Qkm7a4Ee6jnIZgqCjytQ47ztqE";
        DispatcherTimer backupTimer = new DispatcherTimer();
        DispatcherTimer countDownTimer = new DispatcherTimer();
        public static string ServerPath; 
        public static string ServerFile;
        public static string JavaPath;
        public static string JavaFile;

        public MainWindow()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Locate your Minecraft Server";
            dialog.Filter = "Minecraft Server JAR|*.jar";
            if (dialog.ShowDialog() != true) { Close(); return; }
            ServerPath = System.IO.Path.GetDirectoryName(dialog.FileName);
            ServerFile = System.IO.Path.GetFileName(dialog.FileName);
            var dialogjava = new Microsoft.Win32.OpenFileDialog();
            dialogjava.Title = "Locate your java installation";
            dialogjava.Filter = "Java Virtual Machine|*.exe";
            if (dialogjava.ShowDialog() != true) { Close(); return; }
            JavaPath = System.IO.Path.GetDirectoryName(dialogjava.FileName);
            JavaFile = System.IO.Path.GetFileName(dialogjava.FileName);
            InitializeComponent();
            TimeSpan countDownSpan = new TimeSpan(0, 0, 0, 60);
            countDownTimer.Interval = countDownSpan;
            countDownTimer.Tick += countDownTimer_Tick;
            backupTimer.Tick += backupTimer_Tick;
            this.Closed += MainWindow_Closed;
            PlayerNames.ItemsSource = MainViewModel.Current.ServerWrapper.Players;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            MainViewModel.Current.ServerWrapper.stopServer();
        }

        void countDownTimer_Tick(object sender, EventArgs e)
        {
            MainViewModel.Current.ServerWrapper.stopServer();
            MainViewModel.Current.BackupManager.createBackup();
            MainViewModel.Current.ServerWrapper.startServer();
            LastBackup.Text = DateTime.Now.ToString();
            countDownTimer.Stop();
        }

        void backupTimer_Tick(object sender, EventArgs e)
        {
            MainViewModel.Current.ServerWrapper.ServerProc.StandardInput.WriteLine("/say Time for a backup, bitches! You have 1 minute left.");
            countDownTimer.Start();
        }

        private void ServerProc_Exited(object sender, EventArgs e)
        {
            return;
        }

        void ServerProc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // You have to do this through the Dispatcher because this method is called by a different Thread
            Dispatcher.Invoke(new Action(() =>
            {
                ConsoleTextBlock.Text += e.Data + "\r\n";
                ConsoleScroll.ScrollToEnd();
                MainViewModel.Current.ServerWrapper.ParseServerInput(e.Data);
            }));
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan backupSpan = new TimeSpan(0, Int32.Parse(Hours.Text), Int32.Parse(Minutes.Text), 0);
            backupTimer.Interval = backupSpan;
            MainViewModel.Current.ServerWrapper.serverInitialize();
            MainViewModel.Current.ServerWrapper.ServerProc.ErrorDataReceived += new DataReceivedEventHandler(ServerProc_ErrorDataReceived);
            MainViewModel.Current.ServerWrapper.startServer();
            backupTimer.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Current.ServerWrapper.stopServer();
            backupTimer.Stop();
        }

        private void CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Current.BackupManager.createBackup();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Input.Text != null)
            {
                MainViewModel.Current.ServerWrapper.ServerProc.StandardInput.WriteLine(Input.Text);
                Input.Text = "";
            }
        }


    }
}
