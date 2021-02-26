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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;


        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();

            // Handle PropertyChanged events from the ViewModel
            _viewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "IsSchedulerRunning":
                        // UI will be updated, must be on main thread
                        this.Dispatcher.Invoke(() => IsSchedulerRunning_Change(_viewModel.IsSchedulerRunning));
                        break;
                    case "WallpaperChangeTime":
                        this.Dispatcher.Invoke(() => WallpaperChangeTime_Change(_viewModel.WallpaperChangeTime));
                        break;
                    case "CurrentWallpaperName":
                        this.Dispatcher.Invoke(() => CurrentWallpaperName_Change(_viewModel.CurrentWallpaperName));
                        break;
                    case "Location":
                        this.Dispatcher.Invoke(() => Location_Change(_viewModel.Location));
                        break;
                }
            };

            // We subscribe to ViewModel's PropertyChanged event after it's created, so we're not notified of
            // properties set during construction, so we initialize views based on ViewModel here
            InitToViewModel();
        }


        // Private methods

        private void SelectWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectWallpaperButton_Click();
        }

        private void IsSchedulerRunning_Change(bool? isRunning)
        {
            if (isRunning == null)
            {
                statusBar.Background = (Brush)new BrushConverter().ConvertFrom("#FF007acc");
                wallpaperChangeTimeTextBlock.Visibility = Visibility.Collapsed;
                wallpaperNameTextBlock.Visibility = Visibility.Collapsed;
                ((App)Application.Current).SetNotifyIconText("Dynamic Wallpaper");
            }
            else if ((bool)isRunning)
            {
                statusBar.Background = Brushes.Green;
                statusTextBlock.Text = "Running";
                wallpaperChangeTimeTextBlock.Visibility = Visibility.Visible;
                wallpaperNameTextBlock.Visibility = Visibility.Visible;
                ((App)Application.Current).SetNotifyIconText("Dynamic Wallpaper\nRunning");
            }
            else
            {
                statusBar.Background = Brushes.Firebrick;
                statusTextBlock.Text = "Not Running";
                wallpaperChangeTimeTextBlock.Visibility = Visibility.Collapsed;
                wallpaperNameTextBlock.Visibility = Visibility.Collapsed;
                ((App)Application.Current).SetNotifyIconText("Dynamic Wallpaper\nNot running");
            }
        }

        private void WallpaperChangeTime_Change(DateTime time)
        {
            wallpaperChangeTimeTextBlock.Text = $"Next wallpaper change: {time.ToString()}";
            wallpaperChangeTimeTextBlock.Visibility = Visibility.Visible;
        }

        private void CurrentWallpaperName_Change(string name)
        {
            wallpaperNameTextBlock.Text = $"Current wallpaper: {name}";
            wallpaperNameTextBlock.Visibility = Visibility.Visible;
        }

        private void Location_Change(Location location)
        {
            // Update locationTextBlock to show location
            locationTextBlock.Text = $"{location.Latitude}\u00B0N, {location.Longitude}\u00B0E";
        }

        private void WallpaperNameTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((TextBlock)sender).Background = (Brush)new BrushConverter().ConvertFrom("#44FFFFFF");
        }

        private void WallpaperNameTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((TextBlock)sender).Background = (Brush)new BrushConverter().ConvertFrom("#22FFFFFF");
            _viewModel.WallpaperNameTextBlock_Click();
        }

        private void WallpaperNameTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Background = (Brush)new BrushConverter().ConvertFrom("#22FFFFFF");
        }

        private void WallpaperNameTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Background = (Brush)new BrushConverter().ConvertFrom("#00FFFFFF");
        }

        private void InitToViewModel()
        {
            // We can just call the "handler" for every property that can change
            IsSchedulerRunning_Change(_viewModel.IsSchedulerRunning);
            WallpaperChangeTime_Change(_viewModel.WallpaperChangeTime);
            CurrentWallpaperName_Change(_viewModel.CurrentWallpaperName);
            Location_Change(_viewModel.Location);
        }
    }
}
