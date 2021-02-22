using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaper
{
    class MainWindowViewModel
    {
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


        // Private variables

        private WallpaperScheduler _scheduler;

        // Constructor
        public MainWindowViewModel()
        {
            _scheduler = new WallpaperScheduler();
        }
    }
}
