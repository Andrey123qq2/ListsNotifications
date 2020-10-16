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
        static SPUser svcUserForEmptyResponse;
        public static string[] GetCodesFromDeptCodeField(this IERItem item, string DeptCodeFieldName)
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

        public static List<SPPrincipal> GetGroupsByDeptCodeField(this IERItem item, string DeptCodeFieldName, string GroupSuffix)
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

        public static List<SPPrincipal> GetUsersFromUsersFields(this IERItem item, List<string> usersFields, bool valueAfter = true )
        {
            List<SPPrincipal> fieldsPrincipals = new List<SPPrincipal>();
            string userLogin;
            dynamic fieldValue;
            if (svcUserForEmptyResponse == null)
            {
                svcUserForEmptyResponse = item.listItem.Web.EnsureUser("app@sharepoint");
            }

            foreach (string fieldTitle in usersFields)
            {
                fieldValue = item.GetFieldValue(fieldTitle);
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
                            try
                            {
                                principal = item.listItem.ParentList.ParentWeb.SiteGroups.GetByName(userLogin);
                            }
                            catch
                            {
                                continue;
                            }
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
        public static dynamic GetFieldValue(this IERItem item, string fieldTitle, bool valueAfter = true)
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

            if (item.eventType.Contains("ing") && valueAfter)
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

            if (ChangedFieldValue == null)
            {
                ChangedFieldValue = String.Empty;
            }

            return ChangedFieldValue;
        }

        public static List<SPPrincipal> GetRelatedItemUsers(this IERItem item)
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
    }
}
