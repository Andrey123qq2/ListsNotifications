using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;

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

                MainInitNotif.InitItemUpdating(properties);
            }
            catch (Exception ex)
            {
                throw new Exception("CustomER Exception (ItemUpdating): " + properties.ListId + ", " + properties.ListItemId + ", " + "[ " + ex.ToString() + "].");
            }
            finally
            {
                base.EventFiringEnabled = true;
            }
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);

            try
            {
                base.EventFiringEnabled = false;

                MainInitNotif.InitItemAdded(properties);
            }
            catch (Exception ex)
            {
                throw new Exception("CustomER Exception (ItemAdded): " + properties.ListId + ", " + properties.ListItemId + ", " + "[ " + ex.ToString() + "].");
            }
            finally
            {
                base.EventFiringEnabled = true;
            }
        }

        public override void ItemAttachmentAdded(SPItemEventProperties properties)
        {
            base.ItemAttachmentAdded(properties);
            try
            {
                base.EventFiringEnabled = false;

                MainInitNotif.InitItemAttachmentAdded(properties);
            }
            catch (Exception ex)
            {
                throw new Exception("CustomER Exception (ItemAttachmentAdding): " + properties.ListId + ", " + properties.ListItemId + ", " + "[ " + ex.ToString() + "].");
            }
            finally
            {
                base.EventFiringEnabled = true;
            }
        }
    }
}