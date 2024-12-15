namespace BlazorApp_TeleCRM.Service
{
    public interface ITimeZoneService
    {
        DateTime ToLocalTime(DateTime utcDateTime);
        string ToLocalTimeString(DateTime utcDateTime, string format = "yyyy-MM-dd HH:mm:ss");
    }
    public class TimeZoneService : ITimeZoneService
    {
        private readonly TimeZoneInfo _timeZone;
        public TimeZoneService(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone;
        }

        public DateTime ToLocalTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _timeZone);
        }

        public string ToLocalTimeString(DateTime utcDateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return ToLocalTime(utcDateTime).ToString(format);
        }
    }
}
