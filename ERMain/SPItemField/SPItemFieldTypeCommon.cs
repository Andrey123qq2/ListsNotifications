using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class SPItemFieldTypeCommon : SPItemField
    {
        public SPItemFieldTypeCommon(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            FieldValueBeforeToStringForCompare = (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? FieldValueBefore.ToString() : "";
            FieldValueAfterToStringForCompare = (FieldValueAfter != null && FieldValueAfter.ToString() != "") ? FieldValueAfter.ToString() : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            friendlyFieldValue = fieldValueString;
        }
    }
}
