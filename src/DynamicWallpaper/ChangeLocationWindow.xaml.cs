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
    /// Interaction logic for ChangeLocationWindow.xaml
    /// </summary>
    public partial class ChangeLocationWindow : Window
    {
        // Properties
        public double Latitude
        {
            get;
            private set;
        }

        public double Longitude
        {
            get;
            private set;
        }

        public bool OkClicked
        {
            get;
            private set;
        }

        // Constructor
        public ChangeLocationWindow(double lat, double lng)
        {
            InitializeComponent();
            // Initialize properties
            Latitude = lat;
            Longitude = lng;
            OkClicked = false;
            // Set TextBox texts
            latitudeTextBox.Text = lat.ToString();
            longitudeTextBox.Text = lng.ToString();
        }


        // Private methods

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate user input
            string latStr = latitudeTextBox.Text;
            string lngStr = longitudeTextBox.Text;

            if (latStr == null || lngStr == null
                || latStr.Equals("") || lngStr.Equals(""))
            {
                Close();
                return;
            }

            // Parse user input to doubles
            double lat = 123;
            double lng = 322;
            try
            {
                lat = Double.Parse(latitudeTextBox.Text);
                lng = Double.Parse(longitudeTextBox.Text);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
                Close();
                return;
            }

            // Set properties and close
            Latitude = lat;
            Longitude = lng;
            OkClicked = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
