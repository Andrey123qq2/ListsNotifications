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

                if (!SPCommon.IsUpdatingBySystem(properties))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        MainInit.Notifications(properties);
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CustomEventReceiver, ItemUpdating(): Exception: [" + ex.ToString() + "].");
            }
            finally
            {
                base.EventFiringEnabled = true;
            }
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            try
            {
                base.EventFiringEnabled = false;

                if (!SPCommon.IsUpdatingBySystem(properties))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        MainInit.Notifications(properties);
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CustomEventReceiver, ItemUpdated(): Exception: [" + ex.ToString() + "].");
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

                if (!SPCommon.IsUpdatingBySystem(properties) && !SPCommon.IsJustCreated(properties))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        MainInit.NotificationsAttachment(properties);
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CustomEventReceiver, ItemAttachmentAdding(): Exception: [" + ex.ToString() + "].");
            }
            finally
            {
                base.EventFiringEnabled = true;
            }
        }
    }
}