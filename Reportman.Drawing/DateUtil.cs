using System;

namespace Reportman.Drawing
{
    /// <summary>
    /// Provide utitilies about handling DateTime values
    /// </summary>
    public class DateUtil
    {
        public static DateTime FIRST_DELPHI_DAY = new DateTime(1899, 12, 30);
        /// <summary>
        /// Converts a double representing the number of days from 30 Dec 1899 to DateTime
        /// </summary>
        public static DateTime DelphiDateToDateTime(double avalue)
        {
            return FIRST_DELPHI_DAY.Add(DateUtil.DelphiDateTimeToTimeSpan(avalue));
        }

        /// <summary>
        /// Converts a DateTime to a double value representing the number of days from 30 Dec 1899
        /// </summary>
        public static double DateTimeToDelphiDate(DateTime avalue)
        {
            TimeSpan difdate = avalue - FIRST_DELPHI_DAY;
            return difdate.Days + difdate.Hours / 24 + difdate.Minutes / (24 * 60) + difdate.Seconds / (24 * 60 * 60);
        }
        /// <summary>
        /// Calculates the sql literal date value, to include it in sql sentences
        /// </summary>
        /// <param name="valor"></param>
        /// <returns>The sql representation, with quotes of the date (not including time information)</returns>
        public static string DateToSqlLiteral(DateTime value)
        {
            return StringUtil.QuoteStr(value.ToString("yyyy-MM-dd"));
        }
        /// <summary>
        /// Converts a Delphi DateTime to a TimeSpan, time since 30 DEC 1899
        /// </summary>
        public static TimeSpan DelphiDateTimeToTimeSpan(double avalue)
        {
            int days = (int)avalue;
            double atime = avalue - (int)avalue;
            int seconds = (int)(atime * 86400);
            int hours = (int)(seconds / 3600);
            seconds = seconds - hours * 3600;
            int minutes = (int)(seconds / 60);
            seconds = seconds - minutes * 60;
            return new TimeSpan(days, hours, minutes, seconds);
        }
        public static bool IsDateTime(string val, out DateTime result)
        {
            result = DateTime.MinValue;
            // 
            if ((val.Length < 8) || (val.Length > 10))
                return false;
            bool aresult = false;
            aresult = DateTime.TryParse(val, out result);
            if ((result.Year < 1800) || (result.Year >= 9000))
                aresult = false;
            return aresult;
        }
        public static DateTime NextSaturday(DateTime value)
        {
            DateTime result = value;
            while (result.DayOfWeek != DayOfWeek.Saturday)
                result = result.AddDays(1);
            return result;
        }
        public static DateTime NextFriDay(DateTime value)
        {
            DateTime result = value;
            while (result.DayOfWeek != DayOfWeek.Friday)
                result = result.AddDays(1);
            return result;
        }
        public static DateTime NextDayOfMonth(DateTime value, int nday)
        {
            int dayofmon = value.Day;
            if (dayofmon <= nday)
            {
                return value.AddDays(nday - dayofmon);
            }
            else
            {
                value = value.AddMonths(1);
                dayofmon = value.Day;
                if (dayofmon <= nday)
                {
                    return value.AddDays(nday - dayofmon);
                }
                else
                    return value.AddDays(-(dayofmon - nday));
            }
        }
        public static DateTime LastDayOfMonth(DateTime value)
        {
            DateTime dtTo = value;


            dtTo = dtTo.AddMonths(1);
            dtTo = dtTo.AddDays(-(dtTo.Day));

            return dtTo;
        }
        public static DateTime AddWorkableDays(DateTime value, int days)
        {
            while (days > 0)
            {
                if ((value.DayOfWeek != DayOfWeek.Saturday) && (value.DayOfWeek != DayOfWeek.Sunday))
                {
                    days--;
                }
                value = value.AddDays(1);
            }
            return value;
        }
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                     new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }

}
