using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SPSCommon.SPCustomExtensions;

namespace ListsNotifications
{
    class SPItemFieldTypeSPFieldUserValueCollection : SPItemField
    {
        public SPItemFieldTypeSPFieldUserValueCollection(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            SPFieldUserValue[] FieldValueBeforeArr = (FieldValueBefore != null && FieldValueBefore.ToString() != "") ? FieldValueBefore.ToArray() : new SPFieldUserValue[] { };
            SPFieldUserValue[] FieldValueAfterArr = (FieldValueAfter != null && FieldValueAfter.ToString() != "") ? (new SPFieldUserValueCollection(Item.listItem.Web, FieldValueAfter.ToString())).ToArray() : new SPFieldUserValue[] { };

            FieldValueBeforeToStringForCompare = (FieldValueBeforeArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueBeforeArr, p => (p.User != null) ? p.User.LoginName : p.LookupValue )) : "";
            FieldValueAfterToStringForCompare = (FieldValueAfterArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueAfterArr, p => (p.User != null) ? p.User.LoginName : p.LookupValue)) : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            bool valueAfterParam = FriendlyFieldValueAfter == null;
            List<SPPrincipal> fieldPrincipals = valueAfterParam ? Item.GetUsersFromUsersFieldsAfter(new List<string> { FieldTitle }) : Item.listItem.GetUsersFromUsersFields(new List<string> { FieldTitle });
            friendlyFieldValue = String.Join(", ", SPCommon.GetUserNames(fieldPrincipals).ToArray());
        }
    }
}
