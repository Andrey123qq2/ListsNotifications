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
            FieldValueBeforeToStringForCompare = 
                (FieldValueBefore != null && FieldValueBefore.ToString() != "") ?
                Regex.Replace( ((string)FieldValueBefore).Replace("\r\n", "\n"), "^<div></div>$", String.Empty ) : 
                "";
            FieldValueAfterToStringForCompare = 
                (FieldValueAfter != null && FieldValueAfter.ToString() != "") ?
                Regex.Replace( ((string)FieldValueAfter).Replace("\r\n", "\n"), "^<div></div>$", String.Empty ) : 
                "";
        }
        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            friendlyFieldValue = Regex.Replace(fieldValueString, @"href=""/", @"href=""" + Item.listItem.Web.Site.Url + "/");
        }
    }
}