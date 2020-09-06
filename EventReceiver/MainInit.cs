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
    class MainInit
    {
        public static void Notifications(SPItemEventProperties properties)
        {
            ERItem itemER = new ERItem(properties);

            if (itemER.listItem == null)
            {
                return;
            }

            NotificationsTrackFields(itemER);
            NotificationsSingleField(itemER);
        }

        public static void NotificationsTrackFields(ERItem itemER)
        {
            MailItem mailToNotify = new MailItem(itemER, itemER.TrackSPItemFields);
            mailToNotify.SendMail(itemER.listItem.ParentList.ParentWeb);
        }

        public static void NotificationsSingleField(ERItem itemER)
        {
            if (itemER.eventProperties.EventType.ToString().Contains("Attachment"))
            {
                return;
            }

            foreach (KeyValuePair<SPItemField, string> trackField in itemER.TrackSingleMailSPItemFields)
            {
                MailItem mailToNotifySingleField = new MailItem(itemER, new List<SPItemField> { trackField.Key }, trackField.Value, false);
                mailToNotifySingleField.SendMail(itemER.listItem.ParentList.ParentWeb);
            }
        }
    }
}
