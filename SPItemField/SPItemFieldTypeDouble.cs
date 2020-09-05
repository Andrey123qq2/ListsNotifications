using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ListsNotifications
{
    class SPItemFieldTypeDouble : SPItemField
    {
        public SPItemFieldTypeDouble(ERItem itemParam, string fieldTitleParam, bool valueAfterParam = true) : base(itemParam, fieldTitleParam, valueAfterParam)
        {

        }
        public override void GetFieldValuesToStringForCompare()
        {
            fieldValueBeforeToStringForCompare = (fieldValueBefore != null) ? fieldValueBefore.ToString() : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null) ? fieldValueAfter.ToString() : "";
            fieldValueAfterToStringForCompare = Regex.Replace(fieldValueAfterToStringForCompare, @"\.", ",");
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            if (item.listItem.ParentList.Fields.GetField(fieldTitle).FieldRenderingControl.AlternateTemplateName == "PercentageNumberField")
            {
                fieldValueString = fieldValueString.Replace(@",", ".");
                friendlyFieldValue = item.listItem.ParentList.Fields.GetField(fieldTitle).GetFieldValueAsText(fieldValueString);
            }
            else
            {
                friendlyFieldValue = Regex.Replace(fieldValueString, @"\.", ",");
            }
        }
    }
}
