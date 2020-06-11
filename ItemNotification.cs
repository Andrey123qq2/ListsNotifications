using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;

namespace ListsNotifications
{
    class ItemNotification
    {
        public static void Notifications(SPItemEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.WebUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPListItem item;
                    List<SPPrincipal> fieldsAssignees = new List<SPPrincipal>();

                    try
                    {
                        item = web.Lists[properties.ListId].GetItemById(properties.ListItemId);
                    }
                    catch
                    {
                        item = properties.ListItem;
                    }
                    item.SetEventProperties(properties);

                    ConfigParams ERConfig = new ConfigParams(item.ParentList);

                    //get UserNotifyFields
                    List<SPPrincipal> NotifyUsers = item.GetUsersFromUsersFields(ERConfig.UserNotifyFields);
                    
                    MailNotification mailToNotify = new MailNotification(item, ERConfig.TrackFields, NotifyUsers);
                    mailToNotify.SendMail();

                    //ERConfig.TrackFields;

                    //get TrackFields

                    //get users from UserFields (additionally get emails)

                    //get collection of changedFields with old and new values, and SPFields (maybe create class with constructor)

                    //create mail message 
                    //mail body: create from html template (maybe with old and new values by fields)
                    //add to mail all useremails
                    //send mail
                }
            }
        }
        public static bool IsUpdatingBySystem(SPItemEventProperties properties)
            {
                if (properties.UserDisplayName == "app@sharepoint" || properties.UserDisplayName.Contains("svc_"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
    }


}
