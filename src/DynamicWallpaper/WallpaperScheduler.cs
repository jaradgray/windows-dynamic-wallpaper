using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    class WallpaperScheduler : INotifyPropertyChanged
    {
        // Implement notifying PropertyChanged functionality
        public event PropertyChangedEventHandler PropertyChanged; // required by INotifyPropertyChanged interface
        // The method we'll call when we want to raise the PropertyChanged event
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


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

        private bool? _isRunning = null;
        public bool? IsRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                if (value != _isRunning)
                {
                    _isRunning = value;
                    OnPropertyChanged(); // raise PropertyChanged event
                }
            }
        }

        private DateTime _nextChangeTime = DateTime.Now; // time the wallpaper is scheduled to change next
        public DateTime NextChangeTime
        {
            get
            {
                return _nextChangeTime;
            }
            private set
            {
                if (value.Ticks != _nextChangeTime.Ticks)
                {
                    _nextChangeTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _wallpaperName = "";
        public string WallpaperName
        {
            get
            {
                return _wallpaperName;
            }
            private set
            {
                if (!value.Equals(_wallpaperName))
                {
                    _wallpaperName = value;
                    OnPropertyChanged();
                }
            }
        }

        private Location _location;
        public Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (value.Equals(_location))
                {
                    _location = value;
                    OnPropertyChanged();
                }
            }
        }


        // Private variables
        private DynamicWallpaper _wallpaper;
        private Timer _timer;


        // Constructor
        public WallpaperScheduler(double lat, double lng)
        {
            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += Timer_Elapsed;

            _location = new Location(lat, lng);

            Application.Current.Startup += Application_Startup;
            Application.Current.Exit += Application_Exit;
            SystemEvents.TimeChanged += SystemEvents_TimeChanged;
        }


        // Event handlers

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Console.WriteLine("Application.Startup event fired");

            // If current wallpaper's path doesn't match that of last persisted image...
            string current = DesktopManager.GetDesktopWallpaperPath();
            string persisted = Properties.Settings.Default.LastSetWallpaperPath;
            if (!current.Equals(persisted))
            {
                // Indicate we're not scheduling wallpaper changes and return
                IsRunning = false;
                return;
            }

            // Set DirPath to persisted wallpaper's directory
            DirPath = Path.GetDirectoryName(persisted);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SystemEvents.TimeChanged -= SystemEvents_TimeChanged; // unsubscribe to static event
            Console.WriteLine("Unsubscribed from SystemEvents.TimeChanged");
        }

        private void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            SyncToSunProgress();
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
                //MessageBox.Show(message);
                IsRunning = false;
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
                //MessageBox.Show(message);
                IsRunning = false;
                return;
            }

            WallpaperName = _wallpaper.Name;

            // Set wallpaper based on sun's current progress
            SyncToSunProgress();
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

            // Persist current wallpaper's path via Settings
            Properties.Settings.Default.LastSetWallpaperPath = path;
            Properties.Settings.Default.Save(); // persist settings across application sessions

            // Schedule _timer to run when the sun reaches the next image's progress
            Index = (Index + 1) % _wallpaper.Images.Count; // set Index to next image's index
            DateTime changeTime = SunCalcHelper.GetNextTime(_wallpaper.Images[Index].Progress, DateTime.Now, Location.Latitude, Location.Longitude);
            double interval = (changeTime.Ticks - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond;
            if (interval < 1)
            {
                interval = 1; // fire timer immediately if it should have fired in the past
            }
            _timer.Interval = interval;
            IsRunning = true;
            NextChangeTime = changeTime;
        }

        /// <summary>
        /// Sets Index to that of the image whose progress is closest to sun's current
        /// progress without exceeding it, and changes wallpaper to that image
        /// </summary>
        private void SyncToSunProgress()
        {
            _timer.Enabled = false;

            if (_wallpaper == null)
            {
                IsRunning = false;
                return;
            }

            // Set Index to that of the image whose progress is closest to sun's current progress without exceeding it
            DateTime now = DateTime.Now;
            double currentProgress = SunCalcHelper.GetSunProgress(now, Location.Latitude, Location.Longitude);
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
    }
}
