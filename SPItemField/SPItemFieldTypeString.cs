using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            fieldValueBeforeToStringForCompare = (fieldValueBefore != null) ? ((string)fieldValueBefore).Replace("\r\n", "\n") : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null) ? ((string)fieldValueAfter).Replace("\r\n", "\n") : "";
        }
        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            friendlyFieldValue = fieldValueString;
        }
    }
}
