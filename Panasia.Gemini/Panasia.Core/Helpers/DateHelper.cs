using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    public static class DateHelper
    {
        public static DateTime Today = DateTime.Now.Date;

        #region property MinDate
        private static DateTime _MinDate = new DateTime(1900, 1, 1);
        public static DateTime MinDate
        {
            get
            {
                return _MinDate;
            }
        }
        #endregion //property MinDate

        public static bool IsMinDate(this DateTime date)
        {
            return date.Equals(MinDate);
        }
    }
}
