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
        public SPItemFieldTypeSPFieldUserValue(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            SPFieldUserValue fieldValueBeforeUser = new SPFieldUserValue(item.listItem.Web, fieldValueBefore.ToString());
            SPFieldUserValue fieldValueAfterUser = new SPFieldUserValue(item.listItem.Web, fieldValueAfter.ToString());

            fieldValueBeforeToStringForCompare = 
                (fieldValueBefore != null && fieldValueBefore.ToString() != "") ? 
                    (fieldValueBeforeUser.User != null ? fieldValueBeforeUser.User.LoginName : fieldValueBeforeUser.LookupValue) : "";
            fieldValueAfterToStringForCompare = 
                (fieldValueAfter != null && fieldValueAfter.ToString() != "") ?
                    (fieldValueAfterUser.User != null ? fieldValueAfterUser.User.LoginName : fieldValueAfterUser.LookupValue) : "";

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
