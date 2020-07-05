using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Helper
{
   public static class DateHelper
    {
        public const string IncassoDateGridFormat = "dd-MM-yyyy";
        public const string IncassoDateInputFormat = "dd/MM/yyyy";

        public static string ToGridFormat(this DateTime dateTime)
        {
            return dateTime.ToString(IncassoDateGridFormat);
        }
    }
}
