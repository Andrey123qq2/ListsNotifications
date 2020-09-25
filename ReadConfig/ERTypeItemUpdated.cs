using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
	class ERTypeItemUpdated : ERItem
	{
		public ERTypeItemUpdated(SPItemEventProperties properties) : base(properties)
		{
			SetSPItemFieldsAttributesByERType();
		}
		public override void SetSPItemFieldsAttributesByERType()
		{
			this.SetAttribute(listItem.ParentList, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);

			TrackSingleMailSPItemFields = TrackFieldsSingleMail
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f.Key))
				.Where(t => t.IsChanged)
				.ToDictionary(t => t, t => TrackFieldsSingleMail[t.fieldTitle]);
		}

		public override void SendNotifications()
		{
			NotificationsSingleField();
		}
	}
}
