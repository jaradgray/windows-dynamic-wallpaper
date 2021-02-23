using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// This class provides the functionality that makes the wallpaper "dynamic".
    /// Keeps track of the current wallpaper image, schedules image changes
    /// </summary>
    class WallpaperScheduler
    {
        // Properties and their backing fields
        private string _dirPath;

        public string DirPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                DirPath_Change();
            }
        }

        public int Index { get; private set; } // index of the currently displayed ProgressImage in _wallpaper.Images


        // Private variables
        private DynamicWallpaper _wallpaper;
        private Timer _timer;


        // Constructor
        public WallpaperScheduler()
        {
            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += Timer_Elapsed;
        }


        // Private methods

        private void DirPath_Change()
        {
            _timer.Enabled = false; // stop timer

            // Read manifest file
            string json = "";
            try
            {
                json = File.ReadAllText(_dirPath + Path.DirectorySeparatorChar + "manifest.json");
            }
            catch (Exception e)
            {
                string message = $"WallpaperScheduler.DirPath_Change - {e.ToString()}";
                Console.Error.Write(message);
                MessageBox.Show(message);
                return;
            }

            // Instantiate DynamicWallpaper object from json
            try
            {
                _wallpaper = new DynamicWallpaper(json);
            }
            catch (Exception e)
            {
                string message = $"WallpaperScheduler.DirPath_Change - {e.ToString()}";
                Console.Error.Write(message);
                MessageBox.Show(message);
                return;
            }

            // Get path to image that should be set as current wallpaper
            // TODO get sun's current progress
            double currentProgress = -1;
            for (int i = 0; i < _wallpaper.Images.Count; i++)
            {
                double progress = _wallpaper.Images[i].Progress;
                if (progress > currentProgress) break;
                Index = i;
            }

            // change wallpaper
            _timer.Interval = 1;
            _timer.Enabled = true;
        }

        public void Timer_Elapsed(Object source, ElapsedEventArgs e)
        {
            // Set wallpaper to the image at the current Index
            string path = Path.Combine(_dirPath, _wallpaper.Images[Index].Name);
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File doesn't exist: {path}");
            }
            DesktopManager.SetDesktopWallpaper(path);

            // TODO persist current wallpaper's path

            Console.WriteLine($"Elapsed event fired at {e.SignalTime}\nIndex: {Index}");
            // TODO schedule _timer to run when the sun reaches the next image's progress
            //_timer.Interval = 1000;
        }
    }
}
