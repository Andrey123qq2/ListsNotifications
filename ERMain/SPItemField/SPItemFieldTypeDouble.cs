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
        public SPItemFieldTypeDouble(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            FieldValueBeforeToStringForCompare = (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? FieldValueBefore.ToString() : "";
            FieldValueAfterToStringForCompare = (FieldValueAfter != null && FieldValueAfter.ToString() != "") ? FieldValueAfter.ToString() : "";
            FieldValueAfterToStringForCompare = Regex.Replace(FieldValueAfterToStringForCompare, @"\.", ",");
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            if (Item.listItem.ParentList.Fields.GetField(FieldTitle).FieldRenderingControl.AlternateTemplateName == "PercentageNumberField")
            {
                fieldValueString = fieldValueString.Replace(@",", ".");
                friendlyFieldValue = Item.listItem.ParentList.Fields.GetField(FieldTitle).GetFieldValueAsText(fieldValueString);
            }
            else
            {
                friendlyFieldValue = Regex.Replace(fieldValueString, @"\.", ",");
            }
        }
    }
}
