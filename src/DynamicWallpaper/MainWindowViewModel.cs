using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicWallpaperNamespace
{
    class MainWindowViewModel : INotifyPropertyChanged
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
                _scheduler.DirPath = value;
            }
        }

        private bool? _isSchedulerRunning = null;
        public bool? IsSchedulerRunning
        {
            get
            {
                return _isSchedulerRunning;
            }
            private set
            {
                if (value != _isSchedulerRunning)
                {
                    _isSchedulerRunning = value;
                    OnPropertyChanged(); // Raise PropertyChanged event
                }
            }
        }

        private DateTime _wallpaperChangeTime;
        public DateTime WallpaperChangeTime
        {
            get
            {
                return _wallpaperChangeTime;
            }
            private set
            {
                if (value.Ticks != _wallpaperChangeTime.Ticks)
                {
                    _wallpaperChangeTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentWallpaperName = "";
        public string CurrentWallpaperName
        {
            get
            {
                return _currentWallpaperName;
            }
            private set
            {
                if (!value.Equals(_currentWallpaperName))
                {
                    _currentWallpaperName = value;
                    OnPropertyChanged();
                }
            }
        }

        private Location _location = new Location(100, 100);
        public Location Location
        {
            get
            {
                return _location;
            }
            private set
            {
                if (!value.Equals(_location))
                {
                    _location = value;
                    _scheduler.Location = value;
                    OnPropertyChanged();
                }
            }
        }


        // Private variables

        private WallpaperScheduler _scheduler;

        // Constructor
        public MainWindowViewModel()
        {
            // Create WallpaperScheduler from persisted location settings
            double lat = Properties.Settings.Default.Latitude;
            double lng = Properties.Settings.Default.Longitude;
            _scheduler = new WallpaperScheduler(lat, lng);
            // Set Location property (AFTER we create _scheduler)
            Location = new Location(lat, lng);

            // Handle PropertyChanged events from _scheduler
            _scheduler.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "IsRunning":
                        IsSchedulerRunning = _scheduler.IsRunning;
                        break;
                    case "NextChangeTime":
                        WallpaperChangeTime = _scheduler.NextChangeTime;
                        break;
                    case "WallpaperName":
                        CurrentWallpaperName = _scheduler.WallpaperName;
                        break;
                }
            };

            // Initialize state to _scheduler (because we subscribe to its events AFTER we construct it)
            IsSchedulerRunning = _scheduler.IsRunning;
            WallpaperChangeTime = _scheduler.NextChangeTime;
            CurrentWallpaperName = _scheduler.WallpaperName;
        }


        // Public methods

        public void SelectWallpaperButton_Click()
        {
            // Show an OpenFileDialog (because .NET doesn't have a dialog to select a directory)
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.InitialDirectory = _scheduler.DirPath;
            if (ofd.ShowDialog() == true)
            {
                // Set DirPath to path of selected file's directory
                DirPath = System.IO.Path.GetDirectoryName(ofd.FileName);
            }
        }

        public void WallpaperName_Click()
        {
            // Open Windows Explorer and show current wallpaper's directory
            Process.Start(_scheduler.DirPath);
        }

        public void Location_Click()
        {
            // Show window to change location
            ChangeLocationWindow w = new ChangeLocationWindow(Location.Latitude, Location.Longitude);
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();

            // If user clicked Ok button (and input was validated)...
            if (w.OkClicked)
            {
                // Persist location values in settings
                Properties.Settings.Default.Latitude = w.Latitude;
                Properties.Settings.Default.Longitude = w.Longitude;
                Properties.Settings.Default.Save();

                // Set Location property (which re-creates _scheduler)
                Location = new Location(w.Latitude, w.Longitude);
            }
        }
    }
}
