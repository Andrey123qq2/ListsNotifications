using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class SPItemFieldTypeSPFieldLookupValueCollection :SPItemField
    {
        public SPItemFieldTypeSPFieldLookupValueCollection(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            FieldValueBeforeToStringForCompare = FieldValueBefore?.ToString() ?? "";
            FieldValueAfterToStringForCompare = FieldValueAfter?.ToString() ?? "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            SPFieldLookupValue[] fieldValueArr = new SPFieldLookupValueCollection(fieldValueString).ToArray();
            friendlyFieldValue = String.Join(",", Array.ConvertAll(fieldValueArr, p => p.LookupValue));
        }
    }
}
