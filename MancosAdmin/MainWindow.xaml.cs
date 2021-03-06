﻿
using MancosAdmin.Model;
using MancosAdmin.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static string ServerPath;
        public static string ServerFile;
        public static string JavaPath;
        public static string JavaFile;
        public static DispatcherTimer GUINextBackup {get; set;}



        public MainWindow()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Locate your Minecraft Server";
            dialog.Filter = "Minecraft Server JAR|*.jar";
            dialog.InitialDirectory="C:\\Users\\Gregorio\\Documents\\MinecraftServer";
            if (dialog.ShowDialog() != true) { Close(); return; }
            ServerPath = System.IO.Path.GetDirectoryName(dialog.FileName);
            ServerFile = System.IO.Path.GetFileName(dialog.FileName);
            var dialogjava = new Microsoft.Win32.OpenFileDialog();
            dialogjava.Title = "Locate your java installation";
            dialogjava.Filter = "Java Virtual Machine|*.exe";
            dialogjava.InitialDirectory="C:\\Program Files\\Java";
            if (dialogjava.ShowDialog() != true) { Close(); return; }
            JavaPath = System.IO.Path.GetDirectoryName(dialogjava.FileName);
            JavaFile = System.IO.Path.GetFileName(dialogjava.FileName);
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            PlayerNames.ItemsSource = MainViewModel.Current.ServerWrapper.Players;
            Console.ItemsSource = MainViewModel.Current.ServerWrapper.consoleData;
            GUINextBackup = new DispatcherTimer();
            GUINextBackup.Tick += GUINextBackup_Tick;
            KeyDown += MainWindow_KeyDown;
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Input.Text != null)
                {
                    MainViewModel.Current.ServerWrapper.ServerProc.StandardInput.WriteLine(Input.Text);
                    Input.Text = "";
                }
            }
        }

        void GUINextBackup_Tick(object sender, EventArgs e)
        {
            MainViewModel.Current.BackupManager.NextBackupIn = MainViewModel.Current.BackupManager.NextBackupIn.Subtract(new TimeSpan(0, 0, 0, 1));
        }



        void MainWindow_Closed(object sender, EventArgs e)
        {
            MainViewModel.Current.ServerWrapper.stopServer();
        }



        private void ServerProc_Exited(object sender, EventArgs e)
        {
            return;
        }



        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if(MainViewModel.Current.ServerWrapper.Stopped)
            {
                GUINextBackup.Interval = new TimeSpan(0, 0, 1);
                MainViewModel.Current.LaunchServer();
                if (MainViewModel.Current.ServerWrapper.OldVersion)
                {
                    MainViewModel.Current.ServerWrapper.ServerProc.ErrorDataReceived += ServerProc_ErrorDataReceived;
                }
                else
                {
                    MainViewModel.Current.ServerWrapper.ServerProc.OutputDataReceived += ServerProc_OutputDataReceived;
                }
            }
        }

        void ServerProc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // You have to do this through the Dispatcher because this method is called by a different Thread
            Dispatcher.Invoke(new Action(() =>
            {
                ConsoleInput newData = new ConsoleInput(e.Data);
                MainViewModel.Current.ServerWrapper.consoleData.Add(newData);
                Console.ScrollIntoView(newData);
                if (MainViewModel.Current.ServerWrapper.consoleData.Count > 500) MainViewModel.Current.ServerWrapper.consoleData.RemoveAt(0);
                MainViewModel.Current.ServerWrapper.ParseServerInput(e.Data);
            }));
        }

        void ServerProc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // You have to do this through the Dispatcher because this method is called by a different Thread
            Dispatcher.Invoke(new Action(() =>
            {
                ConsoleInput newData = new ConsoleInput(e.Data);
                MainViewModel.Current.ServerWrapper.consoleData.Add(newData);
                Console.ScrollIntoView(newData);
                if (MainViewModel.Current.ServerWrapper.consoleData.Count > 500) MainViewModel.Current.ServerWrapper.consoleData.RemoveAt(0);
                MainViewModel.Current.ServerWrapper.ParseServerInput(e.Data);
            }));
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if(!MainViewModel.Current.ServerWrapper.Stopped)
            {
                MainViewModel.Current.StopServer();
                GUINextBackup.Stop();
            }
        }

        private void CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            if(MainViewModel.Current.ServerWrapper.Stopped)
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
