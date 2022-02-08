using System;
using System.Globalization;

namespace sysinfo
{
    static class Date
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Date);

        public static void Update()
        {
            CultureInfo ci = new CultureInfo("de-DE");
            DateTimeFormatInfo dtfi = ci.DateTimeFormat;
            Calendar cal = ci.Calendar;
            CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dow = ci.DateTimeFormat.FirstDayOfWeek;
            DateTimeOffset now = DateTimeOffset.Now;
            int kw = cal.GetWeekOfYear(now.DateTime, cwr, dow);
            string date = $"{dtfi.GetAbbreviatedDayName(now.DayOfWeek)}{Util.SmallSpace}{now.Day.ToString("00")}{Util.SmallSpace}{dtfi.GetAbbreviatedMonthName(now.Month)}{Util.SmallSpace}{now.Year}";
            string time = $"{now.Hour.ToString("00")}:{now.Minute.ToString("00")}{Util.SmallSpace}{now.Offset.Hours.ToString("+00;-00")}";
            Value = $"{Util.SmallSpace}KW{Util.SmallSpace}{kw}{Util.SmallSpace}{date}{Util.SmallSpace}{time}";
        }
    }
}