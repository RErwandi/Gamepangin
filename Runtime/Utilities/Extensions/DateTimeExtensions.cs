using System;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for DateTime type
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convert Unix Timestamp to DateTime.
        /// </summary>
        /// <param name="unixTime">Unix timestamp to convert.</param>
        /// <returns>Returns converted value of Unix Timestamp.</returns>
        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (long) (unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }
        
        /// <summary>
        /// Convert DateTime to Unix Timestamp.
        /// </summary>
        /// <param name="dateTime">DateTime to convert.</param>
        /// <returns>Returns converted value of DateTime.</returns>
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double) unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }
    }
}