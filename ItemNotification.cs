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
                    if (item == null)
                    {
                        return;
                    }

                    item.SetEventProperties(properties);

                    ConfigParams ERConfig = new ConfigParams(item.ParentList);

                    List<SPPrincipal> NotifyUsers = item.GetUsersFromUsersFields(ERConfig.UserNotifyFields);
                    
                    MailNotification mailToNotify = new MailNotification(item, ERConfig.TrackFields, NotifyUsers);
                    mailToNotify.SendMail();
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
