using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;

namespace ListsNotifications
{
    public static class SPListExtension
    {
        public static List<string> GetListUserFields(this SPList list)
        {
            List<string> arrListUserFields = new List<string>();
            foreach (SPField fieldSP in list.Fields)
            {
                string fTypeName = fieldSP.Type.ToString();
                string fStaticName = fieldSP.StaticName;
                bool notMatch = !Regex.IsMatch(fStaticName, "Editor|PreviouslyAssignedTo");
                if (fTypeName == "User" && notMatch)
                {
                    arrListUserFields.Add(fStaticName);
                }
            }
            return arrListUserFields;
        }
    }
}
