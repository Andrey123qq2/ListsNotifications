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
            SPFieldUserValue fieldValueBeforeUser = new SPFieldUserValue(Item.listItem.Web, FieldValueBefore.ToString());
            SPFieldUserValue fieldValueAfterUser = new SPFieldUserValue();
            if (FieldValueAfter != null)
                fieldValueAfterUser = new SPFieldUserValue(Item.listItem.Web, FieldValueAfter.ToString());

            FieldValueBeforeToStringForCompare = 
                (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? 
                    (fieldValueBeforeUser.User != null ? fieldValueBeforeUser.User.LoginName : fieldValueBeforeUser.LookupValue) : "";
            FieldValueAfterToStringForCompare = 
                (FieldValueAfter != null && FieldValueAfter.ToString() != "") ?
                    (fieldValueAfterUser.User != null ? fieldValueAfterUser.User.LoginName : fieldValueAfterUser.LookupValue) : "";

            if (FieldValueAfter != null && FieldValueAfter.ToString() != "" && FieldValueAfterToStringForCompare == "")
                FieldValueAfterToStringForCompare = new SPFieldUserValue(Item.listItem.Web, FieldValueAfter.ToString()).User.LoginName;
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            try
            {
                friendlyFieldValue = Item.listItem.Web.EnsureUser(new SPFieldUserValue(Item.listItem.Web, fieldValueString.ToString()).LookupValue).Name;
            }
            catch
            {
                friendlyFieldValue = new SPFieldUserValue(Item.listItem.Web, fieldValueString.ToString()).User.Name;
            }
        }
    }
}
