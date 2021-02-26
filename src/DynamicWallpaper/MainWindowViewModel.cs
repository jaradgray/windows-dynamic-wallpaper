using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                    _scheduler = new WallpaperScheduler(value.Latitude, value.Longitude);
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
            Location = new Location(lat, lng);

            // Note: we would instantiate _scheduler from lat and lng here, but we do that when Location property is set

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

        public void WallpaperNameTextBlock_Click()
        {
            // Open Windows Explorer and show current wallpaper's directory
            Process.Start(_scheduler.DirPath);
        }
    }
}
