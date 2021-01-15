using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;
using SPSCommon.SPCustomExtensions;
using SPSCommon.ERItem;

namespace ListsNotifications.EventReceiver1
{
    /// <summary>
    /// События элемента списка
    /// </summary>
    public class EventReceiver1 : SPItemEventReceiver
    {
        /// <summary>
        /// Обновляется элемент.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);

            try
            {
                base.EventFiringEnabled = false;

                if (SPCommon.IsUpdatingByAccountMatch(properties, "svc_") || properties.ListItem == null || SPCommon.IsJustCreated(properties.ListItem))
                    return;

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ERItemNotifications itemER;

                    try { itemER = new ERItemNotificationsItemUpdating(properties); }
                    catch (ERItemListItemNullException) { return; }
                    catch (Exception e) { throw new Exception("ERItem constructor exception: " + e.Message); }

                    NotificationsManager.SendNotifications(itemER);
                });
            }
            catch (Exception ex) { ProcessException(properties, ex); }
            finally { base.EventFiringEnabled = true; }
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);

            try
            {
                base.EventFiringEnabled = false;

                if (SPCommon.IsUpdatingByAccountMatch(properties, "svc_"))
                    return;

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ERItemNotifications itemER;

                    try { itemER = new ERItemNotificationsItemAdded(properties); }
                    catch (ERItemListItemNullException e) { return; }
                    catch (Exception e) { throw new Exception("ERItem constructor exception: " + e.Message); }

                    NotificationsManager.SendNotifications(itemER);
                });
            }
            catch (Exception ex) { ProcessException(properties, ex); }
            finally { base.EventFiringEnabled = true; }
        }

        public override void ItemAttachmentAdded(SPItemEventProperties properties)
        {
            base.ItemAttachmentAdded(properties);
            try
            {
                base.EventFiringEnabled = false;

                if (SPCommon.IsUpdatingByAccountMatch(properties, "svc_") || SPCommon.IsJustCreated(properties.ListItem))
                    return;

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ERItemNotifications itemER;

                    try { itemER = new ERItemNotificationsItemAttachmentAdded(properties); }
                    catch (ERItemListItemNullException) { return; }
                    catch (Exception e) { throw new Exception("ERItem constructor exception: " + e.Message); }

                    NotificationsManager.SendNotifications(itemER);
                });
            }
            catch (Exception ex) { ProcessException(properties, ex); }
            finally { base.EventFiringEnabled = true; }
        }

        private void ProcessException(SPItemEventProperties properties, Exception ex)
        {
            throw new Exception(
                String.Format(
                    "CustomER Exception ({0}): {1}, {2}, " + "[ {3} ].", 
                    properties.EventType, 
                    properties.ListId, 
                    properties.ListItemId, 
                    ex.ToString()
                )
            );
        }
    }
}