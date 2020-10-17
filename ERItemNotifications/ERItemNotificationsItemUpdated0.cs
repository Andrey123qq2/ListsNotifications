using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
	//class ERItemNotificationsItemUpdated : ERItemNotifications
	//{
	//	public ERItemNotificationsItemUpdated(SPItemEventProperties properties) : base(properties)
	//	{
	//		SetSPItemFieldsAttributesByERType();
	//	}
	//	public override void SetSPItemFieldsAttributesByERType()
	//	{
	//		//this.SetAttribute(listItem.ParentList, out TrackFieldsSingleMail, CommonConfigNotif.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);

	//		TrackSingleMailSPItemFields = this.ERConf.ItemUpdatedTrackFields
	//			//.AsParallel()
	//			.Select(f => SPItemFieldFactory.create(this, f.Key))
	//			.Where(t => t.IsChanged)
	//			.ToDictionary(t => t, t => this.ERConf.ItemUpdatedTrackFields[t.fieldTitle]);
	//	}

	//	public override void SendNotifications()
	//	{
	//		NotificationsSingleField();
	//	}
	//}
}
