using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaperNamespace
{
    public struct Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public override bool Equals(object obj)
        {
            return obj is Location
                && ((Location)obj).Latitude == this.Latitude
                && ((Location)obj).Longitude == this.Longitude;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Latitude.GetHashCode();
            hash = (hash * 7) + Longitude.GetHashCode();
            return hash;
        }
    }
}
