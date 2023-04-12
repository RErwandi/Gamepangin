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
        /// <returns></returns>
        public static string DisplayTimer(this float totalSeconds, TimeDisplayFormat format)
        {
            switch (format)
            {
                case TimeDisplayFormat.Second:
                    return DisplayTimerSecond(totalSeconds);
                case TimeDisplayFormat.MinuteSecond:
                        return DisplayTimerMinuteSecond(totalSeconds);
                case TimeDisplayFormat.HourMinuteSecond:
                        return DisplayTimerHourMinuteSecond(totalSeconds);
                case TimeDisplayFormat.DayHourMinuteSecond:
                        return DisplayTimerDayHourMinuteSecond(totalSeconds);
                default:
                    return DisplayTimerDayHourMinuteSecond(totalSeconds);
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

        public static string DisplayTimerFlexible(this float totalSeconds)
        {
            return DisplayFlexible(totalSeconds);
        }
        
        private static string DisplayFlexible(float totalSeconds)
        {
            var timespan = TimeSpan.FromSeconds(totalSeconds);
            var days = (int)timespan.TotalDays;
            var daysText = days > 0 ? $"{days}d " : "";
            var hours = timespan.Hours;
            var hoursText = hours > 0 ? $"{hours}h " : "";
            var minutes = timespan.Minutes;
            var minutesText = minutes > 0 ? $"{minutes}m " : "";
            var seconds = timespan.Seconds;
            var secondsText = seconds > 0 ? $"{seconds}s" : "";
            return $"{daysText}{hoursText}{minutesText}{secondsText}";
        }
    }
}