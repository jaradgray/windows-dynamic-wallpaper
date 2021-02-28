using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaperNamespace
{
    class SunInfoWindowViewModel : INotifyPropertyChanged
    {
        // Implement notifying PropertyChanged functionality
        public event PropertyChangedEventHandler PropertyChanged; // required by INotifyPropertyChanged interface
        // The method we'll call when we want to raise the PropertyChanged event
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Properties and their backing fields

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            private set
            {
                if (value != _date)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        private IEnumerable<SunCalcNet.Model.SunPhase> _phases;
        public IEnumerable<SunCalcNet.Model.SunPhase> Phases
        {
            get
            {
                return _phases;
            }
            private set
            {
                if (!value.Equals(_phases))
                {
                    _phases = value;
                    OnPropertyChanged();
                }
            }
        }


        // Constructor
        public SunInfoWindowViewModel()
        {
            // Initialize properties
            Date = DateTime.Now;
            double lat = Properties.Settings.Default.Latitude;
            double lng = Properties.Settings.Default.Longitude;
            Phases = SunCalcHelper.GetSunPhases(Date, lat, lng);
        }
    }
}
