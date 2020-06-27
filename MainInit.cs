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

            MailItem mailToNotify = new MailItem(itemER, itemER.TrackFields);
            mailToNotify.SendMail(itemER.listItem.ParentList.ParentWeb);

            foreach (KeyValuePair<string, string> trackField in itemER.TrackFieldsSingleMail)
            {
                MailItem mailToNotifySingleField = new MailItem(itemER, new List<string> { trackField.Key }, trackField.Value, false);
                mailToNotifySingleField.SendMail(itemER.listItem.ParentList.ParentWeb);
            }
        }
    }
}
