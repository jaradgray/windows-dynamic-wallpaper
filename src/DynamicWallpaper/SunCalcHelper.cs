using System;
using System.Collections.Generic;
using System.Linq;
using SunCalcNet;
using SunCalcNet.Model;

namespace DynamicWallpaper
{
    /// <summary>
    /// This class provides functionality that requires interacting with SunCalcNet library.
    /// 
    /// Note: Give SunCalc DateTimes in Local time, and convert DateTimes returned by SunCalc
    ///     to Local time, so that we're dealing with time values local to this machine.
    /// </summary>
    public class SunCalcHelper
    {
        // TODO eventually pass lat and lng values to methods instead of hard-coding them here
        public const double LAT = 34.316830;
        public const double LNG = -86.495820;
        public const int PRECISION = 4; // number of decimal places to round to


        #region Public Methods

        /// <summary>
        /// Wrapper for SunCalc.GetSunPhases(), returning all SunPhases in Local time and
        /// implementing workaround for bug mentioned here: https://github.com/mourner/suncalc/issues/11#issue-22960903
        /// </summary>
        /// <param name="date">the day to get sun phases for, represented in Local time</param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="height"></param>
        /// <returns>all SunPhases for the given day, represented in Local time</returns>
        public static IEnumerable<SunPhase> GetSunPhases(DateTime date, double lat, double lng, double height = 0)
        {
            // Report possible error
            if (date.Kind != DateTimeKind.Local)
            {
                Console.Error.WriteLine("SunCalcHelper.GetSunPhases(): date should be in Local time");
            }

            DateTime noon = new DateTime(date.Year, date.Month, date.Day, 12, 0, 0); // noon of the given DateTime's day
            //return SunCalc.GetSunPhases(noon, lat, lng, height); // note: uncommenting this should make tests fail

            // Convert all SunPhases returned by SunCalc to Local time...
            var phases = SunCalc.GetSunPhases(noon, lat, lng, height);
            List<SunPhase> result = new List<SunPhase>();
            foreach (SunPhase phase in phases)
            {
                result.Add(new SunPhase(phase.Name, phase.PhaseTime.ToLocalTime()));
            }
            return result;
        }

        /// <summary>
        /// Returns the sun's progress through the given day as a value in range [0, 360), where 0 is sunrise,
        /// 90 is near solar noon, 180 is sunset, and 270 is near nadir.
        /// </summary>
        /// <param name="date">the date representing the sun's position we want to calculate, in Local time</param>
        /// <returns></returns>
        public static double GetSunProgress(DateTime date)
        {
            // get key timestamps from the given date
            var phases = GetSunPhases(date, LAT, LNG);
            long sunriseTicks = phases.First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
            long sunsetTicks = phases.First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;

            long start = sunriseTicks;
            long end = sunsetTicks;
            long ticks = date.Ticks;

            // we'll calculate a percentage that represents the sun's progress through
            //  the day or night, i.e. only half a full day. We'll convert to a
            //  percentage of progress through a full day by dividing it by 2, and
            //  adding 50% to times during night;
            double nightExtra = 0.0;

            // 3 cases to consider. ticks represents a time that is either:
            //  - before sunriseTicks
            //  - after sunsetTicks
            //  - between sunriseTicks and sunsetTicks

            // adjust start and end if given date isn't between sunrise and sunset
            if (ticks < sunriseTicks)
            {
                // date is during the night, A.M.
                // start = previous day's sunset, end = given date's sunrise
                DateTime prevDay = date.AddTicks(-1 * TimeSpan.TicksPerDay);
                var prevPhases = GetSunPhases(prevDay, LAT, LNG);
                start = prevPhases.First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;
                end = sunriseTicks;
                nightExtra = 0.5;
            }
            else if (ticks >= sunsetTicks)
            {
                // date is during the night, P.M.
                // start = given date's sunset, end = next day's sunrise
                DateTime nextDay = date.AddTicks(TimeSpan.TicksPerDay);
                var nextPhases = GetSunPhases(nextDay, LAT, LNG);
                start = sunsetTicks;
                end = nextPhases.First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
                nightExtra = 0.5;
            }

            // We can now get the percentage that ticks sits in range [start, end], where the range is half a solar day
            double sunProgressAsPercent = GetPercentageInRange(start, end, ticks) / 2 + nightExtra;
            // convert percentage to a value in range [0, 360)
            return Math.Round(sunProgressAsPercent * 360, PRECISION) % 360;
        }

        /// <summary>
        /// Returns the soonest time after the given time that the sun will be at the given progress
        /// </summary>
        /// <param name="deg">sun's progress through the day, represented as a degree value in range [0, 360)</param>
        /// <param name="time">the earliest time to consider</param>
        /// <returns>the soonest time after given time that the sun will be at given progress</returns>
        public static DateTime GetNextTime(double deg, DateTime time)
        {
            double percent;
            long resultTicks = 0;

            if (deg <= 180.0)
            {
                // deg represents sun above horizon
                percent = GetPercentageInRange(0.0, 180.0, deg);
                // get time sun reaches that position between given day's sunrise and sunset
                long minTicks = GetSunPhases(time, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
                long maxTicks = GetSunPhases(time, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;
                resultTicks = GetValueInRangeByPercentage(minTicks, maxTicks, percent);
                // if that time is before given time, return the time the sun reaches that position between the next day's sunrise and sunset
                if (resultTicks < time.Ticks)
                {
                    DateTime nextDay = time.AddTicks(TimeSpan.TicksPerDay);
                    minTicks = GetSunPhases(nextDay, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
                    maxTicks = GetSunPhases(nextDay, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;
                    resultTicks = GetValueInRangeByPercentage(minTicks, maxTicks, percent);
                }
            }
            else
            {
                // deg represents sun below horizon
                percent = GetPercentageInRange(180.0, 360.0, deg);
                // get time sun reaches that position between previous day's sunset and given day's sunrise
                DateTime prevDay = time.AddTicks(-1 * TimeSpan.TicksPerDay);
                long minTicks = GetSunPhases(prevDay, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;
                long maxTicks = GetSunPhases(time, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
                resultTicks = GetValueInRangeByPercentage(minTicks, maxTicks, percent);
                // if that time is before given time, get the time the sun reaches that position between given day's sunset and next day's sunrise
                if (resultTicks < time.Ticks)
                {
                    DateTime nextDay = time.AddTicks(TimeSpan.TicksPerDay);
                    minTicks = GetSunPhases(time, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;
                    maxTicks = GetSunPhases(nextDay, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
                    resultTicks = GetValueInRangeByPercentage(minTicks, maxTicks, percent);
                }
                // if that time is before given time, return the time the sun reaches that position between next day's sunset and day-after-next's sunrise
                if (resultTicks < time.Ticks)
                {
                    DateTime nextDay = time.AddTicks(TimeSpan.TicksPerDay);
                    DateTime dayAfterNext = time.AddTicks(2 * TimeSpan.TicksPerDay);
                    minTicks = GetSunPhases(nextDay, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunset")).PhaseTime.Ticks;
                    maxTicks = GetSunPhases(dayAfterNext, LAT, LNG).First(phase => phase.Name.Value.Equals("Sunrise")).PhaseTime.Ticks;
                    resultTicks = GetValueInRangeByPercentage(minTicks, maxTicks, percent);
                }
            }

            return new DateTime(resultTicks, DateTimeKind.Local);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Returns the percentage @value is in the range [@min, @max]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double GetPercentageInRange(double min, double max, double value)
        {
            return (value - min) / (max - min);
        }

        private static long GetValueInRangeByPercentage(long min, long max, double percent)
        {
            return (long)((max - min) * percent + min);
        }

        #endregion
    }
}
