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
    public class ERItemSPProperties : IERItem
    {
        private SPListItem listitem;
        private SPItemEventProperties eventproperties;
        private string eventtype;
        private string itemtitle;

        public SPListItem listItem
        {
            get
            {
                return listitem;
            }
        }

        public SPItemEventProperties eventProperties
        {
            get
            {
                return eventproperties;
            }
        }
        public string eventType
        {
            get
            {
                return eventtype;
            }
        }
        
        public string itemTitle
        {
            get
            {
                return itemtitle;
            }
        }

        public ERItemSPProperties(SPItemEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.WebUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    List<SPPrincipal> fieldsAssignees = new List<SPPrincipal>();

                    try
                    {
                        listitem = web.Lists[properties.ListId].GetItemById(properties.ListItemId);
                    }
                    catch
                    {
                        listitem = properties.ListItem;
                    }

                    if (listitem == null)
                    {
                        throw new Exception("ListItem not found");
                    }
                }
            }

            eventproperties = properties;

            eventtype = properties.EventType.ToString();

            itemtitle = (listItem.Title != "" && listItem.Title != null) ? listItem.Title : listItem["FileLeafRef"].ToString();
        }

        public ERItemSPProperties()
        { }
    }
}