using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// 
    /// This class's implementation enables the app to run in the background and
    /// shows a NotifyIcon in the notification area
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _doExit;

        protected override void OnStartup(StartupEventArgs e)
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = DynamicWallpaperNamespace.Properties.Resources.AppIcon;
            _notifyIcon.Text = "Dynamic Wallpaper";
            _notifyIcon.Visible = true;

            CreateContextMenu();

            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            base.OnStartup(e);
        }


        // Event Handlers

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_doExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }


        // Public Methods

        public void SetNotifyIconText(string text)
        {
            _notifyIcon.Text = text;
        }


        // Private Methods

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Show Window").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void ExitApplication()
        {
            _doExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
    }
}
