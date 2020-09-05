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
        public SPItemFieldTypeSPFieldLookupValueCollection(ERItem itemParam, string fieldTitleParam, bool valueAfterParam = true) : base(itemParam, fieldTitleParam, valueAfterParam)
        {

        }
        public override void GetFieldValuesToStringForCompare()
        {
            fieldValueBeforeToStringForCompare = (fieldValueBefore != null) ? fieldValueBefore.ToString() : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null) ? (string)fieldValueAfter : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            SPFieldLookupValue[] fieldValueArr = new SPFieldLookupValueCollection(fieldValueString).ToArray();
            friendlyFieldValue = String.Join(",", Array.ConvertAll(fieldValueArr, p => p.LookupValue));
        }
    }
}
