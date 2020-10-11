using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class ERItem<T>
    {
        public SPListItem listItem;
        public SPItemEventProperties eventProperties;
        public readonly string itemTitle;
        public readonly string eventType;

        public T ERConf;

        public ERItem(SPItemEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.WebUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    List<SPPrincipal> fieldsAssignees = new List<SPPrincipal>();

                    try
                    {
                        listItem = web.Lists[properties.ListId].GetItemById(properties.ListItemId);
                    }
                    catch
                    {
                        listItem = properties.ListItem;
                    }

                    if (listItem == null)
                    {
                        throw new Exception("ListItem not found");
                    }
                }
            }

            eventProperties = properties;

            itemTitle = (listItem.Title != "" && listItem.Title != null) ? listItem.Title : listItem["FileLeafRef"].ToString();

            eventType = properties.EventType.ToString();

            ERConf = ERListConf<T>.Get(listItem.ParentList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);
        }
    }
}