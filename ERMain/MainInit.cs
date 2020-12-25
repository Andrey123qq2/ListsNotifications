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
using SPSCommon.ERItem;
using SPSCommon.SPCustomExtensions;

namespace ListsNotifications
{
    internal static class MainInit
    {
        internal static void InitItemUpdating(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingByAccountMatch(properties, "svc_") && properties.ListItem != null && !SPCommon.IsJustCreated(properties.ListItem))
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
        internal static void InitItemAdded(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingByAccountMatch(properties, "svc_"))
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

        internal static void InitItemAttachmentAdded(SPItemEventProperties properties)
        {
            if (!SPCommon.IsUpdatingByAccountMatch(properties, "svc_") && !SPCommon.IsJustCreated(properties.ListItem))
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
