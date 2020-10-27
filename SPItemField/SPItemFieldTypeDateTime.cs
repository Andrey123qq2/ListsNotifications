using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class SPItemFieldTypeDateTime : SPItemField
    {
        public SPItemFieldTypeDateTime(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            dynamic fieldDateTime = item.listItem.ParentList.Fields.GetField(fieldTitle);
            if (fieldDateTime.DisplayFormat.ToString() == "DateOnly" && Regex.IsMatch(fieldValueAfter, @"T00:00:00Z$") && !(item.listItem.ParentList.BaseTemplate == SPListTemplateType.Events || item.listItem.ParentList.BaseTemplate == SPListTemplateType.TasksWithTimelineAndHierarchy))
            {
                fieldValueBefore = (fieldValueBefore != null && fieldValueBefore.ToString() != "") ? fieldValueBefore.ToLocalTime() : null;
            }

            if (item.listItem.ParentList.BaseTemplate == SPListTemplateType.Events || item.listItem.ParentList.BaseTemplate == SPListTemplateType.TasksWithTimelineAndHierarchy)
            {
                fieldValueBeforeToStringForCompare = (fieldValueBefore != null && fieldValueBefore.ToString() != "") ? fieldValueBefore.ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
            }
            else 
            {
                fieldValueBeforeToStringForCompare = (fieldValueBefore != null && fieldValueBefore.ToString() != "") ? fieldValueBefore.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
            }

            fieldValueAfterToStringForCompare = (fieldValueAfter != null && fieldValueAfter.ToString() != "") ? (string)fieldValueAfter : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            if (item.listItem.ParentList.BaseTemplate == SPListTemplateType.Events || item.listItem.ParentList.BaseTemplate == SPListTemplateType.TasksWithTimelineAndHierarchy)
            {
                friendlyFieldValue = DateTime.Parse(fieldValueString.Replace("Z", "")).ToString();
            }
            else
            {
                friendlyFieldValue = fieldValueString.Contains("Z") ? DateTime.Parse(fieldValueString).ToLocalTime().ToString() : DateTime.Parse(fieldValueString).ToString();
            }

            dynamic fieldDateTime = item.listItem.ParentList.Fields.GetField(fieldTitle);
            if (fieldDateTime.DisplayFormat.ToString() == "DateOnly")
            {
                friendlyFieldValue = Regex.Replace(friendlyFieldValue, @"\s[\d:]+$", "");
            }
        }
    }
}
