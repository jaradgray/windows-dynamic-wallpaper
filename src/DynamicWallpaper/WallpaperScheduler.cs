using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// This class provides the functionality that makes the wallpaper "dynamic".
    /// Keeps track of the current wallpaper image, schedules image changes
    /// </summary>
    class WallpaperScheduler
    {
        // Properties and their backing fields
        private string _dirPath;

        public string DirPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                DirPath_Change();
            }
        }

        public int Index { get; private set; } // index of the image to be set as the wallpaper the next time _timer's Elapsed event fires


        // Private variables
        private DynamicWallpaper _wallpaper;
        private Timer _timer;


        // Constructor
        public WallpaperScheduler()
        {
            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += Timer_Elapsed;

            SystemEvents.TimeChanged += SystemEvents_TimeChanged;
            Application.Current.Exit += Application_Exit;
        }

        private void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("SystemEvents.TimeChanged event fired");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SystemEvents.TimeChanged -= SystemEvents_TimeChanged; // unsubscribe to static event
            Console.WriteLine("Unsubscribed from SystemEvents.TimeChanged");
        }


        // Private methods

        private void DirPath_Change()
        {
            _timer.Enabled = false; // stop timer

            // Read manifest file
            string json = "";
            try
            {
                json = File.ReadAllText(_dirPath + Path.DirectorySeparatorChar + "manifest.json");
            }
            catch (Exception e)
            {
                string message = $"WallpaperScheduler.DirPath_Change - {e.ToString()}";
                Console.Error.Write(message);
                MessageBox.Show(message);
                return;
            }

            // Instantiate DynamicWallpaper object from json
            try
            {
                _wallpaper = new DynamicWallpaper(json);
            }
            catch (Exception e)
            {
                string message = $"WallpaperScheduler.DirPath_Change - {e.ToString()}";
                Console.Error.Write(message);
                MessageBox.Show(message);
                return;
            }

            // Get path to image that should be set as current wallpaper
            //  This is the image whose progress is closest to sun's current progress without exceeding it
            DateTime now = DateTime.Now;
            double currentProgress = SunCalcHelper.GetSunProgress(now);
            for (int i = 0; i < _wallpaper.Images.Count; i++)
            {
                double progress = _wallpaper.Images[i].Progress;
                if (progress > currentProgress) break;
                Index = i;
            }

            // Change wallpaper immediately via _timer
            _timer.Interval = 1;
            _timer.Enabled = true;
        }

        public void Timer_Elapsed(Object source, ElapsedEventArgs e)
        {
            // Set wallpaper to the image at the current Index
            string path = Path.Combine(_dirPath, _wallpaper.Images[Index].Name);
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File doesn't exist: {path}");
            }
            DesktopManager.SetDesktopWallpaper(path);

            // TODO persist current wallpaper's path

            // Schedule _timer to run when the sun reaches the next image's progress
            Index = (Index + 1) % _wallpaper.Images.Count; // set Index to next image's index
            DateTime changeTime = SunCalcHelper.GetNextTime(_wallpaper.Images[Index].Progress, DateTime.Now);
            double interval = (changeTime.Ticks - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond;
            if (interval < 1)
            {
                interval = 1; // fire timer immediately if it should have fired in the past
            }
            _timer.Interval = interval;
        }
    }
}
