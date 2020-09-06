using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class SPItemFieldTypeSPFieldUserValue : SPItemField
    {
        public SPItemFieldTypeSPFieldUserValue(ERItem itemParam, string fieldTitleParam, bool valueAfterParam = true) : base(itemParam, fieldTitleParam, valueAfterParam)
        {

        }
        public override void GetFieldValuesToStringForCompare()
        {
            fieldValueBeforeToStringForCompare = (fieldValueBefore != null) ? new SPFieldUserValue(item.listItem.Web, fieldValueBefore.ToString()).User.LoginName : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null && fieldValueAfter.ToString() != "") ? new SPFieldUserValue(item.listItem.Web, fieldValueAfter.ToString()).LookupValue : "";
            if (fieldValueAfter != null && fieldValueAfter.ToString() != "" && fieldValueAfterToStringForCompare == "")
            {
                fieldValueAfterToStringForCompare = new SPFieldUserValue(item.listItem.Web, fieldValueAfter.ToString()).User.LoginName;
            }
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            try
            {
                friendlyFieldValue = item.listItem.Web.EnsureUser(new SPFieldUserValue(item.listItem.Web, fieldValueString.ToString()).LookupValue).Name;
            }
            catch
            {
                friendlyFieldValue = new SPFieldUserValue(item.listItem.Web, fieldValueString.ToString()).User.Name;
            }
        }
    }
}
