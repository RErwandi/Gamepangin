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
        
        public static float HourToMinute(this float hour)
        {
            return hour * 60;
        }
        
        public static float HourToSecond(this float hour)
        {
            return MinuteToSecond(HourToMinute(hour));
        }
    }
}