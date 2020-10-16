using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    public class ERItem<T> : IERItem, IERConf<T>
    {
        private SPListItem _listItem;
        private SPItemEventProperties _eventProperties;
        private string _itemTitle;
        private string _eventType;

        private T _ERConf;

        public SPListItem listItem
        {
            get { return _listItem; }
        }
        public SPItemEventProperties eventProperties
        {
            get { return _eventProperties; }
        }
        public string itemTitle
        {
            get { return _itemTitle; }
        }
        public string eventType
        {
            get { return _eventType; }
        }
        public T ERConf
        {
            get { return _ERConf; }
        }


        public ERItem(SPItemEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.WebUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    List<SPPrincipal> fieldsAssignees = new List<SPPrincipal>();

                    try
                    {
                        _listItem = web.Lists[properties.ListId].GetItemById(properties.ListItemId);
                    }
                    catch
                    {
                        _listItem = properties.ListItem;
                    }

                    if (_listItem == null)
                    {
                        throw new Exception("ListItem not found");
                    }
                }
            }

            _eventProperties = properties;

            _itemTitle = (_listItem.Title != "" && _listItem.Title != null) ? _listItem.Title : _listItem["FileLeafRef"].ToString();

            _eventType = properties.EventType.ToString();

            _ERConf = ERListConf<T>.Get(_listItem.ParentList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);
        }

    }
}