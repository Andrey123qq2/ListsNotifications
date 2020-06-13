using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;

namespace ListsNotifications
{
    public static class SPListItemExtension
    {
        private static SPItemEventProperties eventProperties;

        public static SPItemEventProperties GetEventProperties(this SPListItem item)
        {
            return eventProperties;
        }

        public static void SetEventProperties(this SPListItem item, SPItemEventProperties properties)
        {
            eventProperties = properties;
        }

        public static List<SPPrincipal> GetAssignmentsPrincipals(SPRoleAssignmentCollection assignments)
        {
            List<SPPrincipal> actualAssignees = new List<SPPrincipal>();

            foreach (SPRoleAssignment assignment in assignments)
            {
                if (Regex.IsMatch(assignment.Member.Name, @"svc_|system"))
                {
                    continue;
                }

                foreach (SPRoleDefinition assignmentBinding in assignment.RoleDefinitionBindings)
                {
                    if (assignmentBinding.Name != "Ограниченный доступ")
                    {
                        actualAssignees.Add(assignment.Member);
                    }
                }
            }
            return actualAssignees;
        }

        public static List<SPPrincipal> GetExtraAssignees(this SPListItem item, List<SPPrincipal> principals)
        {
            List<SPPrincipal> extraAssignees = new List<SPPrincipal>();

            List<SPPrincipal> listActualPrincipals = GetAssignmentsPrincipals(item.ParentList.RoleAssignments);
            List<SPPrincipal> itemActualPrincipals = GetAssignmentsPrincipals(item.RoleAssignments);

            List<string> listActualLogins = SPCommon.GetLoginsFromPrincipals(listActualPrincipals);
            List<string> principalsLogins = SPCommon.GetLoginsFromPrincipals(principals);

            foreach (SPPrincipal itemPrincipal in itemActualPrincipals)
            {
                if (!listActualLogins.Contains(itemPrincipal.LoginName) && !principalsLogins.Contains(itemPrincipal.LoginName))
                {
                    extraAssignees.Add(itemPrincipal);
                }
            }

            return extraAssignees;
        }

        public static string[] GetCodesFromDeptCodeField(this SPListItem item, string DeptCodeFieldName)
        {
            dynamic CodeNameValues;
            string[] CodeNames = new String[] { };
            List<string> CodeNamesList = new List<string>();
            string splitPattern = @";#\d+;#";
            string removePattern = @"\d+;#";

            CodeNameValues = item.GetChangedFieldValue(DeptCodeFieldName);
            if (CodeNameValues.GetType().Name == "String")
            {
                if (CodeNameValues == "")
                {
                    return CodeNames;
                }

                CodeNames = Regex.Split(CodeNameValues, splitPattern);
                CodeNames[0] = Regex.Replace(CodeNames[0], removePattern, "");
            }
            if (CodeNameValues.GetType().Name == "SPFieldLookupValueCollection")
            {
                foreach (SPFieldLookupValue CodeValue in CodeNameValues)
                {
                    CodeNamesList.Add(CodeValue.LookupValue);
                    CodeNames = CodeNamesList.ToArray();
                }
            }

            return CodeNames;
        }

        public static List<SPPrincipal> GetGroupsByDeptCodeField(this SPListItem item, string DeptCodeFieldName, string GroupSuffix)
        {
            List<SPPrincipal> codeFieldGroups = new List<SPPrincipal>();
            string[] codeNames;

            if (!item.ParentList.Fields.ContainsField(DeptCodeFieldName))
            {
                return codeFieldGroups;
            }

            codeNames = item.GetCodesFromDeptCodeField(DeptCodeFieldName);

            foreach (string code in codeNames)
            {
                string groupName = code + GroupSuffix;
                SPPrincipal CodeGroup;
                try
                {
                    CodeGroup = item.ParentList.ParentWeb.SiteGroups.GetByName(groupName);
                }
                catch
                {
                    continue;
                }
                codeFieldGroups.Add(CodeGroup);
            }

            return codeFieldGroups;
        }

        public static List<SPPrincipal> GetUsersFromUsersFields(this SPListItem item, List<string> usersFields)
        {
            List<SPPrincipal> fieldsPrincipals = new List<SPPrincipal>();
            string userLogin;
            dynamic fieldValue;
            SPUser svcUserForEmptyResponse = item.Web.EnsureUser("app@sharepoint");

            foreach (string fieldTitle in usersFields)
            {
                if (!item.ParentList.Fields.ContainsField(fieldTitle))
                {
                    continue;
                }

                fieldValue = item.GetChangedFieldValue(fieldTitle);
                if (fieldValue == null || (fieldValue.GetType().Name == "String" && fieldValue == ""))
                {
                    continue;
                }

                if ((fieldValue.GetType().Name == "Int32") || (fieldValue.GetType().Name == "String" && Regex.IsMatch(fieldValue, @"^\d+$")))
                {
                    SPPrincipal principal = item.ParentList.ParentWeb.SiteUsers.GetByID(int.Parse(fieldValue.ToString()));
                    fieldsPrincipals.Add(principal);
                }
                else
                {
                    SPFieldUserValueCollection fieldValueUsers = new SPFieldUserValueCollection(item.Web, fieldValue.ToString());
                    foreach (SPFieldUserValue fieldUser in fieldValueUsers)
                    {
                        SPPrincipal principal;
                        if (fieldUser.User != null && fieldUser.User.LoginName != "")
                        {
                            userLogin = fieldUser.User.LoginName;
                        }
                        else
                        {
                            userLogin = fieldUser.LookupValue;
                        }

                        userLogin = userLogin.Substring(userLogin.IndexOf("\\") + 1);

                        try
                        {
                            principal = item.Web.EnsureUser(userLogin);
                        }
                        catch
                        {
                            principal = item.ParentList.ParentWeb.SiteGroups.GetByName(userLogin);
                        }

                        fieldsPrincipals.Add(principal);
                    }
                }
            }

            if (fieldsPrincipals.Count == 0)
            {
                fieldsPrincipals.Add(svcUserForEmptyResponse);
            }

            return fieldsPrincipals;
        }

        public static dynamic GetChangedFieldValue(this SPListItem item, string fieldTitle)
        {
            dynamic ChangedFieldValue;
            string fieldInternalName;
            string fieldStaticName;

            try
            {
                fieldInternalName = item.ParentList.Fields[fieldTitle].InternalName;
                fieldStaticName = item.ParentList.Fields[fieldTitle].StaticName;
            }
            catch
            {
                fieldInternalName = fieldTitle;
                fieldStaticName = fieldTitle;
            }

            if (SPCommon.IsEventIng(eventProperties))
            {
                ChangedFieldValue = eventProperties.AfterProperties[fieldInternalName];

                if (ChangedFieldValue == null)
                {
                    ChangedFieldValue = eventProperties.AfterProperties[fieldTitle];
                    if (ChangedFieldValue == null)
                    {
                        ChangedFieldValue = eventProperties.AfterProperties[fieldStaticName];
                        if (ChangedFieldValue == null)
                        {
                            ChangedFieldValue = item[fieldTitle];
                        }
                    }
                }
            }
            else
            {
                ChangedFieldValue = item[fieldInternalName];
            }

            return ChangedFieldValue;
        }

        public static bool UserFieldIsChanged(this SPListItem item, string fieldTitle)
        {
            SPFieldUserValue assignedToFieldvalueBefore = new SPFieldUserValue(item.Web, item[fieldTitle].ToString());
            String assignedToLoginBefore = assignedToFieldvalueBefore.User.LoginName;
            assignedToLoginBefore = assignedToLoginBefore.Substring(assignedToLoginBefore.IndexOf("\\") + 1);

            SPFieldUserValue assignedToFieldvalueAfter = new SPFieldUserValue(item.Web, eventProperties.AfterProperties[fieldTitle].ToString());
            String assignedToLoginAfter = assignedToFieldvalueAfter.LookupValue;
            assignedToLoginAfter = assignedToLoginAfter.Substring(assignedToLoginAfter.IndexOf("\\") + 1);

            if (assignedToLoginAfter != "" && assignedToLoginBefore != assignedToLoginAfter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool FieldIsChanged(this SPListItem item, string fieldTitle)
        {
            dynamic FieldValueAfter = item.GetChangedFieldValue(fieldTitle);
            dynamic FieldValueBefore = item[fieldTitle];
            string FieldValueBeforeToString;
            string FieldValueAfterToString;

            switch (item.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name)
            {
                case "DateTime":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? (string)FieldValueAfter : "";
                    break;
                case "Double":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToString() : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? (string)FieldValueAfter : "";
                    break;
                case "SPFieldUserValueCollection":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToString() : "";
                    SPFieldUserValue[] FieldValueBeforeArr = (FieldValueBefore != null) ? FieldValueBefore.ToArray() : new SPFieldUserValue[] { };
                    SPFieldUserValue[] FieldValueAfterArr = (FieldValueAfter != null) ? (new SPFieldUserValueCollection(item.Web, FieldValueAfter.ToString()) ).ToArray() : new SPFieldUserValue[] { };
                    FieldValueBeforeToString = (FieldValueBeforeArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueBeforeArr, p => p.LookupId)) : "";
                    FieldValueAfterToString = (FieldValueAfterArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueAfterArr, p => p.LookupId)) : "";
                    break;
                case "SPFieldUserValue":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? new SPFieldUserValue(item.Web, FieldValueBefore.ToString()).User.LoginName : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? new SPFieldUserValue(item.Web, FieldValueAfter.ToString()).LookupValue : "";
                    if (FieldValueAfter != null && FieldValueAfterToString == "")
                    {
                        FieldValueAfterToString = new SPFieldUserValue(item.Web, FieldValueAfter.ToString()).User.LoginName;
                    }
                    break;
                default:
                    FieldValueBeforeToString = (FieldValueBefore != null) ? (string)FieldValueBefore : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? (string)FieldValueAfter : "";
                    break;
            }


            if ( FieldValueAfterToString != FieldValueBeforeToString )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static List<SPPrincipal> GetRelatedItemUsers(this SPListItem item)
        {
            List<SPPrincipal> arrRealtedItemUsers = new List<SPPrincipal>();
            dynamic relatedItems = item[SPBuiltInFieldId.RelatedItems];
            if (relatedItems == null)
            {
                return arrRealtedItemUsers;
            }
            String relatedItemsString = relatedItems.ToString();

            dynamic jsonRelatedItems = JsonConvert.DeserializeObject(relatedItemsString);

            foreach (dynamic relItem in jsonRelatedItems)
            {
                List<string> arrRelatedListUserFields = new List<string>();
                int relatedItemId = (int)relItem["ItemId"];
                String relatedlistIdString = relItem["ListId"];
                Guid relatedlistId = new Guid(relatedlistIdString);

                SPList relatedList = item.Web.Lists[relatedlistId];
                SPListItem relatedItem = relatedList.GetItemById(relatedItemId);

                arrRelatedListUserFields = relatedList.GetListUserFields();

                arrRealtedItemUsers.AddRange(relatedItem.GetItemUsers(arrRelatedListUserFields));
            }

            return arrRealtedItemUsers;
        }

        public static List<SPUser> GetItemUsers(this SPListItem item, List<string> arrListUserFields)
        {
            List<SPUser> itemUsers = new List<SPUser>();
            foreach (String field in arrListUserFields)
            {
                dynamic fieldUsers = item[field];
                if (fieldUsers == null)
                {
                    continue;
                }
                if (fieldUsers.GetType().Name == "String")
                {
                    int userId = Int16.Parse(fieldUsers.Substring(0, fieldUsers.IndexOf(";")));
                    SPUser user = item.Web.SiteUsers.GetByID(userId);
                    itemUsers.Add(user);
                }
                else
                {
                    foreach (SPFieldUserValue userValue in fieldUsers)
                    {
                        int userId = userValue.LookupId;
                        SPUser user = item.Web.SiteUsers.GetByID(userId);
                        itemUsers.Add(user);
                    }
                }
            };
            return itemUsers;
        }

        public static string GetItemFullUrl(this SPListItem item)
        {
            string itemFullUrl = item.Web.Site.Url + item.ParentList.DefaultDisplayFormUrl + "?ID=" + item.ID;
            return itemFullUrl;
        }

        public static string GetFriendlyChangedFieldValue(this SPListItem item, string fieldTitle)
        {
            string changedFieldValue;
            dynamic FieldValueAfter = item.GetChangedFieldValue(fieldTitle);
            string changedFieldValueOriginal = (FieldValueAfter != null) ? (string)FieldValueAfter : "";

            if (changedFieldValueOriginal == "" || changedFieldValueOriginal == null)
            {
                return "[ValueRemoved]";
            }

            switch (item.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name)
            {
                case "DateTime":
                    changedFieldValue = DateTime.Parse(changedFieldValueOriginal).ToLocalTime().ToString();
                    break;
                case "SPFieldUserValueCollection":
                    List<SPPrincipal> fieldPrincipals = item.GetUsersFromUsersFields(new List<string> { fieldTitle });
                    changedFieldValue = String.Join(", ", SPCommon.GetUserNames(fieldPrincipals).ToArray());
                    break;
                case "SPFieldUserValue":
                    changedFieldValue = item.Web.EnsureUser(new SPFieldUserValue(item.Web, changedFieldValueOriginal.ToString()).LookupValue).Name;
                    break;
                default:
                    changedFieldValue = changedFieldValueOriginal;
                    break;
            }

            return changedFieldValue;
        }
    }
}
