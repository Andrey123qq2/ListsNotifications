using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class ERItemPageSettings : ERItemConfigMethods
    {
        public List<string> TrackFields;
        public Dictionary<string, string> TrackFieldsSingleMail;
        public List<string> ItemCreateFields;

        public ERItemPageSettings(SPList listSP)
        {
            this.SetAttribute(listSP, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);
            this.SetAttribute(listSP, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);
            this.SetAttribute(listSP, out ItemCreateFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMCREATE, true);
        }
    }
}
