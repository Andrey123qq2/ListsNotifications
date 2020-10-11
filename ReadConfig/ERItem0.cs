using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    public class ERItem0 : ERItemSPProperties, IERConf
    {
        private readonly ERConfNotifications erconf;
        public ERConfNotifications ERConf {
            get {
                return erconf;
            }
        }

        public ERItem0(SPItemEventProperties properties) : base(properties)
        {
            erconf = ERConfFactory<ERConfNotifications>.Create(NotifCommonConfig.ROOT_FOLDER_PROPERTY_NAME, listItem.ParentList);
        }

        public ERItem0(SPList List)
        {
            erconf = ERConfFactory<ERConfNotifications>.Create(NotifCommonConfig.ROOT_FOLDER_PROPERTY_NAME, List);
        }
    }
}
