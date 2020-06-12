using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

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

        public static List<string> GetUserNames(List<SPPrincipal> principalsList)
        {
            List<string> userNames = new List<string>();

            foreach (SPPrincipal principal in principalsList)
            {
                if (principal.GetType().Name != "SPUser")
                {
                    continue;
                }
                SPUser user = (SPUser)principal;
                userNames.Add(user.Name);
            }

            return userNames;
        }

        public static List<string> GetUserMails(List<SPPrincipal> principalsList)
        {
            List<string> toMailsList = new List<string>();

            foreach (SPPrincipal principal in principalsList)
            {
                if (principal.GetType().Name != "SPUser")
                {
                    continue;
                }
                SPUser user = (SPUser)principal;
                toMailsList.Add(user.Email);
            }

            return toMailsList;
        }
    }
}
