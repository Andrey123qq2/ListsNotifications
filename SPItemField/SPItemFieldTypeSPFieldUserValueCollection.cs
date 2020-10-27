using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class SPItemFieldTypeSPFieldUserValueCollection : SPItemField
    {
        public SPItemFieldTypeSPFieldUserValueCollection(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            SPFieldUserValue[] FieldValueBeforeArr = (fieldValueBefore != null && fieldValueBefore.ToString() != "") ? fieldValueBefore.ToArray() : new SPFieldUserValue[] { };
            SPFieldUserValue[] FieldValueAfterArr = (fieldValueAfter != null && fieldValueAfter.ToString() != "") ? (new SPFieldUserValueCollection(item.listItem.Web, fieldValueAfter.ToString())).ToArray() : new SPFieldUserValue[] { };

            fieldValueBeforeToStringForCompare = (FieldValueBeforeArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueBeforeArr, p => (p.User != null) ? p.User.LoginName : p.LookupValue )) : "";
            fieldValueAfterToStringForCompare = (FieldValueAfterArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueAfterArr, p => p.LookupValue)) : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            bool valueAfterParam = friendlyFieldValueAfter == null;
            List<SPPrincipal> fieldPrincipals = valueAfterParam ? item.GetUsersFromUsersFieldsAfter(new List<string> { fieldTitle }) : item.listItem.GetUsersFromUsersFields(new List<string> { fieldTitle });
            friendlyFieldValue = String.Join(", ", SPCommon.GetUserNames(fieldPrincipals).ToArray());
        }
    }
}
