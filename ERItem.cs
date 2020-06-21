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
    public class ERItem : ERItemConfigParams
    {
        public SPListItem listItem;
        public SPItemEventProperties eventProperties;

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
                        return;
                    }
                }
            }

            eventProperties = properties;
            this.SetConfigAttributes(listItem.ParentList);
        }
    }
}
