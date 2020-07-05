using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Helper
{
    public static class DateHelper
    {
        public const string IncassoDateFormat = "dd/MM/yyyy HH:mm:ss";
        public const string IncassoDateGridFormat = "dd-MM-yyyy";
        public const string IncassoDateInputFormat = "dd/MM/yyyy";
        public const string IncassoAmountFormat = "{0:N}";
        public const string ddMMyyyFormat = "dd-MM-yyyy";
        public const string MMddyyyy = "MM-dd-yyyy";

        public static List<string> GetFormates()
        {
            return new List<string> { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy"};
        }
        public static string ToIncassoFormat(this float number)
        {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), IncassoAmountFormat, number);
            

        }
        public static string ToIncassoGridFormat(this float? number)
        {
            if (number.HasValue)
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), IncassoAmountFormat, number);
            }
            return string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), IncassoAmountFormat, 0);

        }

        public static string ToIncassoFormat(this double number)
        {
            return string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), IncassoAmountFormat,number);
        }
        public static string ToIncassoGridFormat(this double? number)
        {
            if (number.HasValue)
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), IncassoAmountFormat, number);
            }
            return string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), IncassoAmountFormat, 0);
        }

        public static string ToIncassoFormat(this double? number)
        {
            if(number.HasValue)
            return number.Value.ToString(IncassoAmountFormat);
            return "";
        }

        public static string ToGridDateFormat(this string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime dateTime))
                    return dateTime.ToString(IncassoDateGridFormat);
                else
                {
                    try
                    {
                        var dateTime1 = DateTime.ParseExact(date, IncassoDateFormat, System.Globalization.CultureInfo.InvariantCulture);
                        return dateTime1.ToString(IncassoDateGridFormat);
                    }
                    catch (FormatException)
                    { }
                }
            }
            
            return string.Empty;
        }

        public static string ToGridFormat(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToString(IncassoDateGridFormat);
            return string.Empty;
        }

        public static string ToGridFormat(this DateTime date)
        {
                return date.ToString(IncassoDateGridFormat);
        }

        public static string ToDate(this string date,string format)
        {
            if (DateTime.TryParse(date, out DateTime dateTime))
                return dateTime.ToString(format);
            else
            {
                try
                {
                    var dateTime1 = DateTime.ParseExact(date, IncassoDateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    return dateTime.ToString(format);
                }
                catch (FormatException)
                { }
            }

            return null;
        }

        public static DateTime? ToDate(this string date)
        {
            if (DateTime.TryParse(date, out DateTime dateTime))
                return dateTime;
            else
            {
                try
                {
                    var dateTime1 = DateTime.ParseExact(date, IncassoDateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    return dateTime1;
                }
                catch (FormatException)
                { }
            }

            return null;
        }

    }
}
