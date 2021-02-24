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
                }
            };
        }


        // Private methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = directoryTextBox.Text;
            if (text.Equals(""))
            {
                MessageBox.Show("Directory path can't be empty");
                return;
            }
            _viewModel.DirPath = directoryTextBox.Text;
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
                statusBar.Background = Brushes.Red;
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

        private void WallpaperNameTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.WallpaperNameTextBlock_Click();
        }
    }
}
