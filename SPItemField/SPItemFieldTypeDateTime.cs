using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ListsNotifications
{
    class SPItemFieldTypeDateTime : SPItemField
    {
        public SPItemFieldTypeDateTime(ERItem itemParam, string fieldTitleParam, bool valueAfterParam = true) : base(itemParam, fieldTitleParam, valueAfterParam)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            dynamic fieldDateTime = item.listItem.ParentList.Fields.GetField(fieldTitle);
            if (fieldDateTime.DisplayFormat.ToString() == "DateOnly" && Regex.IsMatch(fieldValueAfter, @"T00:00:00Z$"))
            {
                fieldValueBefore = (fieldValueBefore != null) ? fieldValueBefore.ToLocalTime() : null;
            }

            fieldValueBeforeToStringForCompare = (fieldValueBefore != null) ? fieldValueBefore.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null) ? (string)fieldValueAfter : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            friendlyFieldValue = DateTime.Parse(fieldValueString).ToLocalTime().ToString();

            dynamic fieldDateTime = item.listItem.ParentList.Fields.GetField(fieldTitle);
            if (fieldDateTime.DisplayFormat.ToString() == "DateOnly")
            {
                friendlyFieldValue = Regex.Replace(friendlyFieldValue, @"\s[\d:]+$", "");
            }
        }
    }
}
