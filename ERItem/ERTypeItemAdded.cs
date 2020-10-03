using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
	class ERTypeItemAdded : ERItemNotifHandler
	{
		public ERTypeItemAdded(SPItemEventProperties properties) : base(properties)
		{
			SetSPItemFieldsAttributesByERType();
		}
		public override void SetSPItemFieldsAttributesByERType()
		{
			//this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMADDED, true);
			TrackFields = ERConf.ItemAddedTrackFields;

			TrackSPItemFields = TrackFields
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f, false))
				.Where(t => t.IsChanged)
				.ToList();
		}

		public override void SendNotifications()
		{
			NotificationsTrackFields();
		}
	}
}
