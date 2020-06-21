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

            MailNotification mailToNotify = new MailNotification(itemER);
            mailToNotify.SendMail(itemER.listItem.ParentList.ParentWeb);
        }
    }
}
