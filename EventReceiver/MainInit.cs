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
        public static void Init(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingBySystem(properties) && !SPCommon.IsJustCreated(properties))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    Notifications(properties);
                });
            }
        }
        public static void Notifications(SPItemEventProperties properties)
        {

            ERItem itemER;

            try
            {
                itemER = ERItemFactory.Create(properties);
            }
            catch (Exception e)
            {
                throw new Exception("ERItem constructor exception: " + e.Message);
            }

            if (itemER.NotifiersPresent == false)
            {
                return;
            }

            itemER.SendNotifications();

            //if (itemER.eventType.Contains("Attachment"))
            //{

            //}

        }
        //public static void Notifications(SPItemEventProperties properties)
        //{
        //    ERItem itemER;
        //    try
        //    {
        //        itemER = new ERItem(properties);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("ERItem constructor exception: " + e.Message);
        //    }

        //    if (itemER.NotifiersPresent == false)
        //    {
        //        return;
        //    }

        //    NotificationsTrackFields(itemER);
        //    NotificationsSingleField(itemER);
        //}

        //public static void NotificationsAttachment(SPItemEventProperties properties)
        //{
        //    ERItem itemER;
        //    try
        //    {
        //        itemER = new ERItem(properties);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("ERItem constructor exception: " + e.Message);
        //    }

        //    //ERItem itemER = new ERItem(properties);

        //    //if (itemER.listItem == null)
        //    //{
        //    //    return;
        //    //}

        //    NotificationsAttachments(itemER);
        //}

        //public static void NotificationsTrackFields(ERItem itemER)
        //{
        //    if (itemER.TrackSPItemFields.Count == 0)
        //    {
        //        return;
        //    }

        //    MailItem mailToNotify = new MailItem(itemER, itemER.TrackSPItemFields, "", itemER.eventType.Contains("ing"));
        //    mailToNotify.SendMail(itemER.listItem.ParentList.ParentWeb);
        //}

        //public static void NotificationsSingleField(ERItem itemER)
        //{
        //    foreach (KeyValuePair<SPItemField, string> trackField in itemER.TrackSingleMailSPItemFields)
        //    {
        //        MailItem mailToNotifySingleField = new MailItem(itemER, new List<SPItemField> { trackField.Key }, trackField.Value, false);
        //        mailToNotifySingleField.SendMail(itemER.listItem.ParentList.ParentWeb);
        //    }
        //}

        //public static void NotificationsAttachments(ERItem itemER)
        //{
        //    MailItem mailToNotify = new MailItem(itemER);
        //    mailToNotify.SendMail(itemER.listItem.ParentList.ParentWeb);
        //}
    }
}
