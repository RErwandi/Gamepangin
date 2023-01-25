using System;

namespace Gamepangin
{
    public static class TimeExtension
    {
        public static float SecondToMinute(this float second)
        {
            return second / 60;
        }

        public static float SecondToHour(this float second)
        {
            return SecondToMinute(second) / 60;
        }

        public static float MinuteToSecond(this float minute)
        {
            return minute * 60;
        }

        public static float MinuteToHour(this float minute)
        {
            return minute / 60;
        }
        
        public static float HourToMinute(this float hour)
        {
            return hour * 60;
        }
        
        public static float HourToSecond(this float hour)
        {
            return MinuteToSecond(HourToMinute(hour));
        }

        public enum TimeDisplayFormat { Second, MinuteSecond, HourMinuteSecond, DayHourMinuteSecond }
        /// <summary>
        /// Display total seconds into readable time display formats.
        /// </summary>
        /// <param name="totalSeconds">total seconds</param>
        /// <param name="format">Time format to display</param>
        /// <param name="flexible">if true, zero will not be shown. example: show "16m 24s" instead of "0h 16m 24s"</param>
        /// <returns></returns>
        public static string DisplayTimer(this float totalSeconds, TimeDisplayFormat format, bool flexible = false)
        {
            var timespan = TimeSpan.FromSeconds(totalSeconds);
            
            switch (format)
            {
                case TimeDisplayFormat.Second:
                    return DisplayTimerSecond(totalSeconds);
                case TimeDisplayFormat.MinuteSecond:
                    if(timespan.TotalMinutes > 1 || !flexible)
                        return DisplayTimerMinuteSecond(totalSeconds);
                    return DisplayTimerSecond(totalSeconds);
                case TimeDisplayFormat.HourMinuteSecond:
                    if(timespan.TotalHours > 1 || !flexible)
                        return DisplayTimerHourMinuteSecond(totalSeconds);
                    if(timespan.TotalMinutes > 1)
                        return DisplayTimerMinuteSecond(totalSeconds);
                    return DisplayTimerSecond(totalSeconds);
                case TimeDisplayFormat.DayHourMinuteSecond:
                    if(timespan.TotalDays > 1 || !flexible)
                        return DisplayTimerDayHourMinuteSecond(totalSeconds);
                    if(timespan.TotalHours > 1)
                        return DisplayTimerHourMinuteSecond(totalSeconds);
                    if(timespan.TotalMinutes > 1)
                        return DisplayTimerMinuteSecond(totalSeconds);
                    return DisplayTimerSecond(totalSeconds);
                default:
                    return DisplayTimerHourMinuteSecond(totalSeconds);
            }
        }

        private static string DisplayTimerSecond(float totalSeconds)
        {
            var timespan = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)timespan.TotalSeconds}s";
        }
        
        private static string DisplayTimerMinuteSecond(float totalSeconds)
        {
            var timespan = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)timespan.TotalMinutes:D2}m {timespan.Seconds:D2}s";
        }
        
        private static string DisplayTimerHourMinuteSecond(float totalSeconds)
        {
            var timespan = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)timespan.TotalHours:D2}h {timespan.Minutes:D2}m {timespan.Seconds:D2}s";
        }
        
        private static string DisplayTimerDayHourMinuteSecond(float totalSeconds)
        {
            var timespan = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)timespan.TotalDays:D2}d {timespan.Hours:D2}h {timespan.Minutes:D2}m {timespan.Seconds:D2}s";
        }
    }
}