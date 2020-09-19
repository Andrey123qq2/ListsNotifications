using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;

namespace ListsNotifications
{
    public class ERItemSPProperties : ERItemConfigMethods
    {
        public SPListItem listItem;
        public SPItemEventProperties eventProperties;
        public string eventType;
        public string itemTitle;

        public ERItemSPProperties(SPItemEventProperties properties)
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

            eventType = properties.EventType.ToString();

            itemTitle = (listItem.Title != "" && listItem.Title != null) ? listItem.Title : listItem["FileLeafRef"].ToString();
        }

        public ERItemSPProperties()
        { }
    }
}