﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class SPItemFieldTypeString : SPItemField
    {
        public SPItemFieldTypeString(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            fieldValueBeforeToStringForCompare = (fieldValueBefore != null && fieldValueBefore.ToString() != "") ? ((string)fieldValueBefore).Replace("\r\n", "\n") : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null && fieldValueAfter.ToString() != "") ? ((string)fieldValueAfter).Replace("\r\n", "\n") : "";
        }
        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            friendlyFieldValue = Regex.Replace(fieldValueString, @"href=""/", @"href=""" + item.listItem.Web.Site.Url + "/");

            //friendlyFieldValue = fieldValueString;
        }
    }
}