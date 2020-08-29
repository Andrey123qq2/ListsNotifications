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
    public static class ERItemSPExtension
    {
        public static string[] GetCodesFromDeptCodeField(this ERItem item, string DeptCodeFieldName)
        {
            dynamic CodeNameValues;
            string[] CodeNames = new String[] { };
            List<string> CodeNamesList = new List<string>();
            string splitPattern = @";#\d+;#";
            string removePattern = @"\d+;#";

            CodeNameValues = item.GetFieldValue(DeptCodeFieldName);
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

        public static List<SPPrincipal> GetGroupsByDeptCodeField(this ERItem item, string DeptCodeFieldName, string GroupSuffix)
        {
            List<SPPrincipal> codeFieldGroups = new List<SPPrincipal>();
            string[] codeNames;

            if (!item.listItem.ParentList.Fields.ContainsField(DeptCodeFieldName))
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
                    CodeGroup = item.listItem.ParentList.ParentWeb.SiteGroups.GetByName(groupName);
                }
                catch
                {
                    continue;
                }
                codeFieldGroups.Add(CodeGroup);
            }

            return codeFieldGroups;
        }

        public static List<SPPrincipal> GetUsersFromUsersFields(this ERItem item, List<string> usersFields, bool valueAfter = true )
        {
            List<SPPrincipal> fieldsPrincipals = new List<SPPrincipal>();
            string userLogin;
            dynamic fieldValue;
            SPUser svcUserForEmptyResponse = item.listItem.Web.EnsureUser("app@sharepoint");

            foreach (string fieldTitle in usersFields)
            {
                //if (!item.listItem.ParentList.Fields.ContainsField(fieldTitle))
                //{
                //    continue;
                //}

                fieldValue = item.GetFieldValue(fieldTitle, valueAfter);
                if (fieldValue == null || (fieldValue.GetType().Name == "String" && fieldValue == ""))
                {
                    continue;
                }

                if ((fieldValue.GetType().Name == "Int32") || (fieldValue.GetType().Name == "String" && Regex.IsMatch(fieldValue, @"^\d+$")))
                {
                    SPPrincipal principal = item.listItem.ParentList.ParentWeb.SiteUsers.GetByID(int.Parse(fieldValue.ToString()));
                    fieldsPrincipals.Add(principal);
                }
                else
                {
                    SPFieldUserValueCollection fieldValueUsers = new SPFieldUserValueCollection(item.listItem.Web, fieldValue.ToString());
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
                            principal = item.listItem.Web.EnsureUser(userLogin);
                        }
                        catch
                        {
                            principal = item.listItem.ParentList.ParentWeb.SiteGroups.GetByName(userLogin);
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

        //TO ERItem !!
        public static dynamic GetFieldValue(this ERItem item, string fieldTitle, bool valueAfter = true)
        {
            dynamic ChangedFieldValue;
            string fieldInternalName;
            string fieldStaticName;

            try
            {
                fieldInternalName = item.listItem.ParentList.Fields[fieldTitle].InternalName;
                fieldStaticName = item.listItem.ParentList.Fields[fieldTitle].StaticName;
            }
            catch
            {
                fieldInternalName = fieldTitle;
                fieldStaticName = fieldTitle;
            }

            if (SPCommon.IsEventIng(item.eventProperties) && valueAfter)
            {
                ChangedFieldValue = item.eventProperties.AfterProperties[fieldInternalName];

                if (ChangedFieldValue == null)
                {
                    ChangedFieldValue = item.eventProperties.AfterProperties[fieldTitle];
                    if (ChangedFieldValue == null)
                    {
                        ChangedFieldValue = item.eventProperties.AfterProperties[fieldStaticName];
                        if (ChangedFieldValue == null)
                        {
                            ChangedFieldValue = item.listItem[fieldTitle];
                        }
                    }
                }
            }
            else
            {
                ChangedFieldValue = item.listItem[fieldInternalName];
            }

            return ChangedFieldValue;
        }

        //TO ERItem !! TODO: remove if no needed
        //public static bool UserFieldIsChanged(this ERItem item, string fieldTitle)
        //{
        //    SPFieldUserValue assignedToFieldvalueBefore = new SPFieldUserValue(item.listItem.Web, item.listItem[fieldTitle].ToString());
        //    String assignedToLoginBefore = assignedToFieldvalueBefore.User.LoginName;
        //    assignedToLoginBefore = assignedToLoginBefore.Substring(assignedToLoginBefore.IndexOf("\\") + 1);

        //    SPFieldUserValue assignedToFieldvalueAfter = new SPFieldUserValue(item.listItem.Web, item.eventProperties.AfterProperties[fieldTitle].ToString());
        //    String assignedToLoginAfter = assignedToFieldvalueAfter.LookupValue;
        //    assignedToLoginAfter = assignedToLoginAfter.Substring(assignedToLoginAfter.IndexOf("\\") + 1);

        //    if (assignedToLoginAfter != "" && assignedToLoginBefore != assignedToLoginAfter)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public static bool FieldIsChanged(this ERItem item, string fieldTitle)
        {
            dynamic FieldValueAfter = item.GetFieldValue(fieldTitle);
            dynamic FieldValueBefore = item.listItem[fieldTitle];
            string FieldValueBeforeToString;
            string FieldValueAfterToString;

            switch (item.listItem.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name)
            {
                case "DateTime":
                    dynamic fieldDateTime = item.listItem.ParentList.Fields.GetField(fieldTitle);
                    if (fieldDateTime.DisplayFormat.ToString() == "DateOnly" && Regex.IsMatch(FieldValueAfter, @"T00:00:00Z$"))
                    {
                        FieldValueBefore = (FieldValueBefore != null) ? FieldValueBefore.ToLocalTime() : null;
                    }

                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? (string)FieldValueAfter : "";
                    break;
                case "Double":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToString() : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? FieldValueAfter.ToString() : "";
                    FieldValueAfterToString = Regex.Replace(FieldValueAfterToString, @"\.", ",");
                    break;
                case "SPFieldUserValueCollection":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToString() : "";
                    SPFieldUserValue[] FieldValueBeforeArr = (FieldValueBefore != null) ? FieldValueBefore.ToArray() : new SPFieldUserValue[] { };
                    SPFieldUserValue[] FieldValueAfterArr = (FieldValueAfter != null) ? (new SPFieldUserValueCollection(item.listItem.Web, FieldValueAfter.ToString()) ).ToArray() : new SPFieldUserValue[] { };
                    FieldValueBeforeToString = (FieldValueBeforeArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueBeforeArr, p => p.LookupId)) : "";
                    FieldValueAfterToString = (FieldValueAfterArr.Length > 0) ? String.Join(",", Array.ConvertAll(FieldValueAfterArr, p => p.LookupId)) : "";
                    break;
                case "SPFieldUserValue":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? new SPFieldUserValue(item.listItem.Web, FieldValueBefore.ToString()).User.LoginName : "";
                    FieldValueAfterToString = (FieldValueAfter != null && FieldValueAfter.ToString() != "") ? new SPFieldUserValue(item.listItem.Web, FieldValueAfter.ToString()).LookupValue : "";
                    if (FieldValueAfter != null && FieldValueAfter.ToString() != "" && FieldValueAfterToString == "")
                    {
                        FieldValueAfterToString = new SPFieldUserValue(item.listItem.Web, FieldValueAfter.ToString()).User.LoginName;
                    }
                    break;
                case "SPFieldLookupValueCollection":
                    FieldValueBeforeToString = (FieldValueBefore != null) ? FieldValueBefore.ToString() : "";
                    FieldValueAfterToString = (FieldValueAfter != null) ? (string)FieldValueAfter : "";
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


        public static List<SPPrincipal> GetRelatedItemUsers(this ERItem item)
        {
            List<SPPrincipal> arrRealtedItemUsers = new List<SPPrincipal>();
            dynamic relatedItems;
            try
            {
                relatedItems = item.listItem[SPBuiltInFieldId.RelatedItems];
                if (relatedItems == null)
                {
                    relatedItems = item.eventProperties.ListItem[SPBuiltInFieldId.RelatedItems];
                }
            }
            catch
            {
                relatedItems = null;
            }

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

                SPList relatedList = item.listItem.Web.Lists[relatedlistId];
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
                    int userId = int.Parse(fieldUsers.Substring(0, fieldUsers.IndexOf(";")));
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

        public static string GetItemFullUrl(this SPListItem itemSP)
        {
            string itemFullUrl = itemSP.Web.Site.Url + itemSP.ParentList.DefaultDisplayFormUrl + "?ID=" + itemSP.ID;
            return itemFullUrl;
        }

        public static string GetFriendlyFieldValue(this ERItem item, string fieldTitle, bool valueAfter = true)
        {
            string friendlyFieldValue;
            dynamic fieldValue = item.GetFieldValue(fieldTitle, valueAfter);
            string fieldValueString;
            try
            {
                fieldValueString = (fieldValue != null) ? (string)fieldValue : ""; 
            }
            catch
            {
                fieldValueString = (fieldValue != null) ? fieldValue.ToString() : "";
            }
            

            if (fieldValueString == "" || fieldValueString == null)
            {
                return "-";
            }

            switch (item.listItem.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name)
            {
                case "DateTime":
                    friendlyFieldValue = DateTime.Parse(fieldValueString).ToLocalTime().ToString();

                    dynamic fieldDateTime = item.listItem.ParentList.Fields.GetField(fieldTitle);
                    if (fieldDateTime.DisplayFormat.ToString() == "DateOnly")
                    {
                        friendlyFieldValue = Regex.Replace(friendlyFieldValue, @"\s[\d:]+$", "");
                    }
                    
                    break;
                case "Double":
                    friendlyFieldValue = Regex.Replace(fieldValueString, @"\.", ",");
                    break;
                case "SPFieldUserValueCollection":
                    List<SPPrincipal> fieldPrincipals = item.GetUsersFromUsersFields(new List<string> { fieldTitle }, valueAfter);
                    friendlyFieldValue = String.Join(", ", SPCommon.GetUserNames(fieldPrincipals).ToArray());
                    break;
                case "SPFieldUserValue":
                    friendlyFieldValue = item.listItem.Web.EnsureUser(new SPFieldUserValue(item.listItem.Web, fieldValueString.ToString()).LookupValue).Name;
                    break;
                case "SPFieldLookupValueCollection":
                    SPFieldLookupValue[] fieldValueArr = new SPFieldLookupValueCollection(fieldValueString).ToArray();
                    friendlyFieldValue = String.Join( ",", Array.ConvertAll(fieldValueArr, p => p.LookupValue) );
                    break;
                default:
                    friendlyFieldValue = fieldValueString;
                    break;
            }

            return friendlyFieldValue;
        }
    }
}
