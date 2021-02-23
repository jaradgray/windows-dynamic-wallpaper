using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// This class represents a single image within a dynamic wallpaper (a dynamic
    /// wallpaper can have multiple images)
    /// </summary>
    class ProgressImage
    {
        // Properties and backing fields
        public string Name { get; }
        public double Progress { get; }

        // Constructor
        public ProgressImage(string name, double progress)
        {
            Name = name;
            Progress = progress;
        }
    }
}
