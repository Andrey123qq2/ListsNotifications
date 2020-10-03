using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    public class ERItem : ERItemSPProperties, IERConf
    {
        private readonly ERConfNotifications erconf;
        public ERConfNotifications ERConf {
            get {
                return erconf;
            }
        }

        public ERItem(SPItemEventProperties properties) : base(properties)
        {
            erconf = ERConfFactory<ERConfNotifications>.Create(NotifCommonConfig.ROOT_FOLDER_PROPERTY_NAME, listItem.ParentList);
        }

        public ERItem(SPList List)
        {
            erconf = ERConfFactory<ERConfNotifications>.Create(NotifCommonConfig.ROOT_FOLDER_PROPERTY_NAME, List);
        }
    }
}
