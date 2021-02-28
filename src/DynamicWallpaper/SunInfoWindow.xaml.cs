using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// Interaction logic for SunInfoWindow.xaml
    /// </summary>
    public partial class SunInfoWindow : Window
    {
        private SunInfoWindowViewModel _viewModel;

        public SunInfoWindow()
        {
            InitializeComponent();

            _viewModel = new SunInfoWindowViewModel();

            // Observe ViewModel's data
            _viewModel.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "Date":
                        Date_Change(_viewModel.Date);
                        break;
                    case "Phases":
                        Phases_Change(_viewModel.Phases);
                        break;
                }
            };

            // Initialize views to ViewModel data
            Date_Change(_viewModel.Date);
            Phases_Change(_viewModel.Phases);
        }


        // Private methods

        private void Date_Change(DateTime date)
        {
            dateTextBlock.Text = date.ToString("dddd, MMMM d, yyyy");
        }

        private void Phases_Change(IEnumerable<SunCalcNet.Model.SunPhase> phases)
        {
            // Update TextBlocks
            // time
            DateTime sunrise = phases.First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime;
            DateTime sunriseEnd = phases.First(phase => phase.Name.Value.Equals("Sunrise End")).PhaseTime;
            DateTime goldenHourEnd = phases.First(phase => phase.Name.Value.Equals("Golden Hour End")).PhaseTime;
            DateTime solarNoon = phases.First(phase => phase.Name.Value.Equals("Solar Noon")).PhaseTime;
            DateTime goldenHour = phases.First(phase => phase.Name.Value.Equals("Golden Hour")).PhaseTime;
            DateTime sunsetStart = phases.First(phase => phase.Name.Value.Equals("Sunset Start")).PhaseTime;
            DateTime sunset = phases.First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime;
            DateTime dusk = phases.First(phase => phase.Name.Value.Equals("Dusk")).PhaseTime;
            DateTime nauticalDusk = phases.First(phase => phase.Name.Value.Equals("Nautical Dusk")).PhaseTime;
            DateTime night = phases.First(phase => phase.Name.Value.Equals("Night")).PhaseTime;
            DateTime nadir = phases.First(phase => phase.Name.Value.Equals("Nadir")).PhaseTime;
            DateTime nightEnd = phases.First(phase => phase.Name.Value.Equals("Night End")).PhaseTime;
            DateTime nauticalDawn = phases.First(phase => phase.Name.Value.Equals("Nautical Dawn")).PhaseTime;
            DateTime dawn = phases.First(phase => phase.Name.Value.Equals("Dawn")).PhaseTime;

            string timeFormat = "h:mm tt"; // e.g. "5:30 PM"
            sunriseTimeTextBlock.Text = sunrise.ToString(timeFormat);
            sunriseEndTimeTextBlock.Text = sunriseEnd.ToString(timeFormat);
            goldenHourEndTimeTextBlock.Text = goldenHourEnd.ToString(timeFormat);
            solarNoonTimeTextBlock.Text = solarNoon.ToString(timeFormat);
            goldenHourTimeTextBlock.Text = goldenHour.ToString(timeFormat);
            sunsetStartTimeTextBlock.Text = sunsetStart.ToString(timeFormat);
            sunsetTimeTextBlock.Text = sunset.ToString(timeFormat);
            duskTimeTextBlock.Text = dusk.ToString(timeFormat);
            nauticalDuskTimeTextBlock.Text = nauticalDusk.ToString(timeFormat);
            nightTimeTextBlock.Text = night.ToString(timeFormat);
            nadirTimeTextBlock.Text = nadir.ToString(timeFormat);
            nightEndTimeTextBlock.Text = nightEnd.ToString(timeFormat);
            nauticalDawnTimeTextBlock.Text = nauticalDawn.ToString(timeFormat);
            dawnTimeTextBlock.Text = dawn.ToString(timeFormat);

            // progress
            double lat = Properties.Settings.Default.Latitude;
            double lng = Properties.Settings.Default.Longitude;
            sunriseProgressTextBlock.Text = SunCalcHelper.GetSunProgress(sunrise, lat, lng).ToString();
            sunriseEndProgressTextBlock.Text = SunCalcHelper.GetSunProgress(sunriseEnd, lat, lng).ToString();
            goldenHourEndProgressTextBlock.Text = SunCalcHelper.GetSunProgress(goldenHourEnd, lat, lng).ToString();
            solarNoonProgressTextBlock.Text = SunCalcHelper.GetSunProgress(solarNoon, lat, lng).ToString();
            goldenHourProgressTextBlock.Text = SunCalcHelper.GetSunProgress(goldenHour, lat, lng).ToString();
            sunsetStartProgressTextBlock.Text = SunCalcHelper.GetSunProgress(sunsetStart, lat, lng).ToString();
            sunsetProgressTextBlock.Text = SunCalcHelper.GetSunProgress(sunset, lat, lng).ToString();
            duskProgressTextBlock.Text = SunCalcHelper.GetSunProgress(dusk, lat, lng).ToString();
            nauticalDuskProgressTextBlock.Text = SunCalcHelper.GetSunProgress(nauticalDusk, lat, lng).ToString();
            nightProgressTextBlock.Text = SunCalcHelper.GetSunProgress(night, lat, lng).ToString();
            nadirProgressTextBlock.Text = SunCalcHelper.GetSunProgress(nadir, lat, lng).ToString();
            nightEndProgressTextBlock.Text = SunCalcHelper.GetSunProgress(nightEnd, lat, lng).ToString();
            nauticalDawnProgressTextBlock.Text = SunCalcHelper.GetSunProgress(nauticalDawn, lat, lng).ToString();
            dawnProgressTextBlock.Text = SunCalcHelper.GetSunProgress(dawn, lat, lng).ToString();
        }
    }
}
