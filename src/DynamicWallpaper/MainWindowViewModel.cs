using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        // Private variables

        private WallpaperScheduler _scheduler;

        // Constructor
        public MainWindowViewModel()
        {
            _scheduler = new WallpaperScheduler();

            // Handle PropertyChanged events from _scheduler
            _scheduler.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "IsRunning":
                        IsSchedulerRunning = _scheduler.IsRunning;
                        break;
                }
            };
        }
    }
}
