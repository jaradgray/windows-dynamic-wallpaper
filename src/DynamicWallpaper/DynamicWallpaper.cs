using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// This class represents a dynaimic wallpaper object
    /// </summary>
    class DynamicWallpaper
    {
        // Properties and their backing fields
        public string Name { get; }
        public List<ProgressImage> Images { get; }

        // Constructor
        public DynamicWallpaper(string json)
        {
            // Parse json to set properties
            JObject o;
            try
            {
                o = JObject.Parse(json);
            }
            catch (JsonReaderException e)
            {
                string message = $"DynamicWallpaper constructor - {e.ToString()}";
                Console.Error.Write(message);
                MessageBox.Show(message);
                return;
            }

            Name = (string)o["name"];

            // ProgressImages
            Images = new List<ProgressImage>();
            JArray arr = (JArray)o["images"];
            foreach (JObject obj in arr.Children<JObject>())
            {
                string name = (string)obj["name"];
                double progress = (double)obj["progress"];
                if (name == null || progress == null)
                {
                    throw new JsonException("Missing property in images array");
                }
                Images.Add(new ProgressImage(name, progress));
            }
        }
    }
}
