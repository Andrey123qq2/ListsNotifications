using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;
using ListsNotifications.ReadConfig;

namespace ListsNotifications
{
    static class MainInit
    {
        public static void InitItemUpdating(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingBySystem(properties) && properties.ListItem != null && !SPCommon.IsJustCreated(properties.ListItem))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ERItemNotifications itemER;

                    try
                    {
                        itemER = new ERItemNotificationsItemUpdating(properties);
                    }
                    catch (ERItemListItemNullException e) {
                        return;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("ERItem constructor exception: " + e.Message);
                    }

                    itemER.SendNotifications();
                });
            }
        }
        public static void InitItemAdded(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingBySystem(properties))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ERItemNotifications itemER;

                    try
                    {
                        itemER = new ERItemNotificationsItemAdded(properties);
                    }
                    catch (ERItemListItemNullException e)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("ERItem constructor exception: " + e.Message);
                    }

                    itemER.SendNotifications();
                });
            }
        }

        public static void InitItemAttachmentAdded(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingBySystem(properties) && !SPCommon.IsJustCreated(properties.ListItem))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ERItemNotifications itemER;

                    try
                    {
                        itemER = new ERItemNotificationsItemAttachmentAdded(properties);
                    }
                    catch (ERItemListItemNullException e)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("ERItem constructor exception: " + e.Message);
                    }

                    itemER.SendNotifications();
                });
            }
        }
    }
}
