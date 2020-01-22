using System;

namespace RiotAPIAccessLayer.Time
{
    public class TimeUtils
    {
        public static DateTime UnixToDateTime(long unixtime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddMilliseconds(unixtime).ToLocalTime();
            return dt;
        }
    }
}
