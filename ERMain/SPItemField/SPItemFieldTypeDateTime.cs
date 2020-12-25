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
            dynamic fieldDateTime = Item.listItem.ParentList.Fields.GetField(FieldTitle);
            if (fieldDateTime.DisplayFormat.ToString() == "DateOnly" && FieldValueAfter != null && Regex.IsMatch(FieldValueAfter.ToString(), @"T00:00:00Z$") && !(Item.listItem.ParentList.BaseTemplate == SPListTemplateType.Events || Item.listItem.ParentList.BaseTemplate == SPListTemplateType.TasksWithTimelineAndHierarchy))
            {
                FieldValueBefore = (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? FieldValueBefore.ToLocalTime() : null;
            }

            if (Item.listItem.ParentList.BaseTemplate == SPListTemplateType.Events || Item.listItem.ParentList.BaseTemplate == SPListTemplateType.TasksWithTimelineAndHierarchy)
            {
                FieldValueBeforeToStringForCompare = (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? FieldValueBefore.ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
            }
            else 
            {
                FieldValueBeforeToStringForCompare = (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? FieldValueBefore.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
            }

            if (FieldValueAfter != null && FieldValueAfter.GetType().Name != "DateTime")
            {
                FieldValueAfterToStringForCompare = (FieldValueAfter != null && FieldValueAfter.ToString() != "") ? FieldValueAfter.ToString() : "";
            }
            else
            {
                FieldValueAfterToStringForCompare = (FieldValueAfter != null && FieldValueAfter.ToString() != "") ? FieldValueAfter.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
            }
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            if (Item.listItem.ParentList.BaseTemplate == SPListTemplateType.Events || Item.listItem.ParentList.BaseTemplate == SPListTemplateType.TasksWithTimelineAndHierarchy)
            {
                friendlyFieldValue = DateTime.Parse(fieldValueString.Replace("Z", "")).ToString();
            }
            else
            {
                friendlyFieldValue = fieldValueString.Contains("Z") ? DateTime.Parse(fieldValueString).ToLocalTime().ToString() : DateTime.Parse(fieldValueString).ToString();
            }

            dynamic fieldDateTime = Item.listItem.ParentList.Fields.GetField(FieldTitle);
            if (fieldDateTime.DisplayFormat.ToString() == "DateOnly")
            {
                friendlyFieldValue = Regex.Replace(friendlyFieldValue, @"\s[\d:]+$", "");
            }
        }
    }
}
