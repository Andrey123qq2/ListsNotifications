using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class ERItemNotificationsItemUpdating : ERItemNotifications
	{
		public ERItemNotificationsItemUpdating(SPItemEventProperties properties) : base(properties)
		{
			SetSPItemFieldsAttributesByERType();
		}
		public override void SetSPItemFieldsAttributesByERType()
		{
			//this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);

			TrackSPItemFields = this.ERConf.ItemUpdatingTrackFields
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f))
				.Where(t => t.IsChanged)
				.ToList();

            TrackSingleMailSPItemFields = this.ERConf.ItemUpdatingTrackFieldsSingleMail
                //.AsParallel()
                .Select(f => SPItemFieldFactory.create(this, f.Key))
                .Where(t => t.IsChanged)
                .ToDictionary(t => t, t => this.ERConf.ItemUpdatingTrackFieldsSingleMail[t.fieldTitle]);
		}

		public override void SendNotifications()
		{
			NotificationsTrackFields(CommonConfigNotif.MAIL_SUBJECT_ITEMS, CommonConfigNotif.MAIL_MODIFIED_BY_TEMPLATE);
			NotificationsTrackFieldsSingleMail(CommonConfigNotif.MAIL_MODIFIED_BY_TEMPLATE);
		}
	}
}
