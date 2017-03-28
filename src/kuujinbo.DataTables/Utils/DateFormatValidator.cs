using System;

namespace kuujinbo.DataTables.Utils
{
    public static class DateFormatValidator
    {
        public const string BAD_DATE_FORMAT = "unrecognized date format {0}";

        public static bool TryParse(string format)
        {
            DateTime outDate;
            return DateTime.TryParse(DateTime.Now.ToString(format), out outDate)
                ? true : false;
        }
    }
}