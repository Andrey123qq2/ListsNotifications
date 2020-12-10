using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
	class ERItemNotificationsItemAdded : ERItemNotifications
	{
		public ERItemNotificationsItemAdded(SPItemEventProperties properties) : base(properties)
		{
			SetSPItemFieldsAttributesByERType();
		}
		public override void SetSPItemFieldsAttributesByERType()
		{
			//this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMADDED, true);
			//TrackFields = ERConf.ItemAddedTrackFields;
			
			TrackSPItemFields = this.ERConf.ItemAddedTrackFields
				//.AsParallel()
				.Where(f => this.listItem.ParentList.Fields.ContainsField(f))
				.Select(f => SPItemFieldFactory.create(this, f, false))
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
			NotificationsTrackFields(CommonConfigNotif.MAIL_SUBJECT_ITEMS_ADDED, CommonConfigNotif.MAIL_CREATED_BY_TEMPLATE);
			NotificationsTrackFieldsSingleMail(CommonConfigNotif.MAIL_MODIFIED_BY_TEMPLATE);
		}
	}
}
