using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DynamicWallpaperNamespace
{
    /// <summary>
    /// Provides desktop utilities like setting the wallpaper and getting the current wallpaper path
    /// </summary>
    class DesktopManager
    {
        #region Constants
        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;
        #endregion

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfoA(
            int uiAction,
            int uiParam,
            string pvParam,
            int fWinIni
        );


        #region Public Methods

        /// <summary>
        /// Returns the path to the current wallpaper by querying the registry
        /// 
        /// Tried to use SystemParametersInfoA() like SetDesktopWallpaper() and passing SPI_GETDESKWALLPAPER, but it wasn't working
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopWallpaperPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "Wallpaper", 0).ToString();
        }

        /// <summary>
        /// Sets the desktop wallpaper via SystemParametersInfoA() function
        /// 
        /// Based on this SO answer: https://stackoverflow.com/a/1061682
        /// </summary>
        /// <param name="path"></param>
        public static void SetDesktopWallpaper(string path)
        {
            SystemParametersInfoA(
                SPI_SETDESKWALLPAPER,
                0,
                path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE
            );
        }

        #endregion
    }
}
