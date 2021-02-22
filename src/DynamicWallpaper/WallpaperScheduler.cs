using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaper
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


        // Private methods

        private void DirPath_Change()
        {
            // Read manifest file
            string json = "";
            try
            {
                json = File.ReadAllText(_dirPath + Path.DirectorySeparatorChar + "manifest.json");
            }
            catch (Exception e)
            {
                Console.Error.Write($"WallpaperScheduler.DirPath_Change - {e.Message}");
            }

            // get path to image that should be set as current wallpaper
            // change wallpaper
            // schedule next wallpaper change
        }
    }
}
