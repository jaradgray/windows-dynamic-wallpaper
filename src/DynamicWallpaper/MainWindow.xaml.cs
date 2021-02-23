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
                if (e.PropertyName.Equals("IsSchedulerRunning"))
                {
                    // UI will be updated, must be on main thread
                    this.Dispatcher.Invoke(() => IsSchedulerRunning_Change(_viewModel.IsSchedulerRunning));
                }
            };
        }

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
            }
            else if ((bool)isRunning)
            {
                statusBar.Background = Brushes.Green;
                statusTextBlock.Text = "Running";
            }
            else
            {
                statusBar.Background = Brushes.Red;
                statusTextBlock.Text = "Not Running";
            }
        }
    }
}
