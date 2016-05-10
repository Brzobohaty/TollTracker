using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollTracker.model
{
    static class TimeUtils
    {
        public static String DAY_FORMAT = "d. MMMM yyyy";

        public static String MONTH_FORMAT = "MMMM yyyy";

        public static DateTime getStartOfDay(DateTime dateTime)
        {
           return dateTime.Date;
        }

        internal static DateTime getStartOfWeek(DateTime dateTime)
        {
            return dateTime.Date.AddDays(-6);
        }

        internal static DateTime getStartOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        internal static DateTime getEndOfDay(DateTime dateTime)
        {
            return dateTime.Date.AddDays(1);
        }

        internal static DateTime getEndOfWeek(DateTime dateTime)
        {
            return dateTime.Date.AddDays(1);
        }

        internal static DateTime getEndOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}
