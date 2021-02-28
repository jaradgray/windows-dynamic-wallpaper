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

        private Dictionary<string, double> _averageProgresses;
        public Dictionary<string, double> AverageProgresses
        {
            get
            {
                return _averageProgresses;
            }
            private set
            {
                if (!value.Equals(_averageProgresses))
                {
                    _averageProgresses = value;
                    OnPropertyChanged();
                }
            }
        }


        // Instance variables
        private double _lat;
        private double _lng;


        // Constructor
        public SunInfoWindowViewModel()
        {
            // Initialize properties
            Date = DateTime.Now;
            _lat = Properties.Settings.Default.Latitude;
            _lng = Properties.Settings.Default.Longitude;
            Phases = SunCalcHelper.GetSunPhases(Date, _lat, _lng);
            AverageProgresses = CalculateAverageProgresses();
        }


        // Private methods

        /// <summary>
        /// Returns a Dictionary containing the average sun progress values for each SunPhase in the SunCalc library for the next year
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, double> CalculateAverageProgresses()
        {
            // Build a Dictionary that will contain progress totals for each SunPhase
            Dictionary<string, double> progressTotals = new Dictionary<string, double>();
            progressTotals.Add(SunCalcHelper.PHASE_NAME_SUNRISE, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_SUNRISE_END, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_GOLDEN_HOUR_END, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_SOLAR_NOON, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_GOLDEN_HOUR, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_SUNSET_START, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_SUNSET, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_DUSK, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_NAUTICAL_DUSK, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_NIGHT, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_NADIR, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_NIGHT_END, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_NAUTICAL_DAWN, 0);
            progressTotals.Add(SunCalcHelper.PHASE_NAME_DAWN, 0);

            // Build a List of DateTimes containing one DateTime per week for the next 52 weeks
            List<DateTime> days = new List<DateTime>();
            for (int week = 0; week < 52; week++)
            {
                days.Add(Date.AddDays(7 * week));
            }

            // Sum progresses for each Sunphase across all days
            foreach (var day in days)
            {
                var phases = SunCalcHelper.GetSunPhases(day, _lat, _lng);
                foreach (var phase in phases)
                {
                    progressTotals[phase.Name.Value] += SunCalcHelper.GetSunProgress(phase.PhaseTime, _lat, _lng);
                }
            }

            // Build a Dictionary of average progresses by dividing each entry in progressTotals by the number of elements in days
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> entry in progressTotals)
            {
                double avg = progressTotals[entry.Key] / days.Count;
                result.Add(entry.Key, avg);
            }

            return result;
        }
    }
}
