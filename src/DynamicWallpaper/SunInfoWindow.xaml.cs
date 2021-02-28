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
                    case "AverageProgresses":
                        AverageProgresses_Change(_viewModel.AverageProgresses);
                        break;
                }
            };

            // Initialize views to ViewModel data
            Date_Change(_viewModel.Date);
            AverageProgresses_Change(_viewModel.AverageProgresses);
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
            sunriseProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(sunrise, lat, lng), 2).ToString();
            sunriseEndProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(sunriseEnd, lat, lng), 2).ToString();
            goldenHourEndProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(goldenHourEnd, lat, lng), 2).ToString();
            solarNoonProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(solarNoon, lat, lng), 2).ToString();
            goldenHourProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(goldenHour, lat, lng), 2).ToString();
            sunsetStartProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(sunsetStart, lat, lng), 2).ToString();
            sunsetProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(sunset, lat, lng), 2).ToString();
            duskProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(dusk, lat, lng), 2).ToString();
            nauticalDuskProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(nauticalDusk, lat, lng), 2).ToString();
            nightProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(night, lat, lng), 2).ToString();
            nadirProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(nadir, lat, lng), 2).ToString();
            nightEndProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(nightEnd, lat, lng), 2).ToString();
            nauticalDawnProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(nauticalDawn, lat, lng), 2).ToString();
            dawnProgressTextBlock.Text = Math.Round(SunCalcHelper.GetSunProgress(dawn, lat, lng), 2).ToString();
        }

        private void AverageProgresses_Change(Dictionary<string, double> progresses)
        {
            // Update TextBlocks
            sunriseAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_SUNRISE], 2).ToString();
            sunriseEndAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_SUNRISE_END], 2).ToString();
            goldenHourEndAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_GOLDEN_HOUR_END], 2).ToString();
            solarNoonAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_SOLAR_NOON], 2).ToString();
            goldenHourAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_GOLDEN_HOUR], 2).ToString();
            sunsetStartAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_SUNSET_START], 2).ToString();
            sunsetAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_SUNSET], 2).ToString();
            duskAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_DUSK], 2).ToString();
            nauticalDuskAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_NAUTICAL_DUSK], 2).ToString();
            nightAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_NIGHT], 2).ToString();
            nadirAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_NADIR], 2).ToString();
            nightEndAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_NIGHT_END], 2).ToString();
            nauticalDawnAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_NAUTICAL_DAWN], 2).ToString();
            dawnAverageProgressTextBlock.Text = Math.Round(progresses[SunCalcHelper.PHASE_NAME_DAWN], 2).ToString();
        }
    }
}
