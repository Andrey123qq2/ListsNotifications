using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ListsNotifications
{
    class SPCommon
    {
        public static bool IsUTCDateString(string str)
        {
            bool IsUTCDate;
            IsUTCDate = Regex.IsMatch(str, @"^\d{4}(-\d+){2}T[\d\:]+Z$");

            return IsUTCDate;
        }
    }
}
