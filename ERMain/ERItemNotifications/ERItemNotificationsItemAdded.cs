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
		}
		public override void SetSPItemFieldsByERType()
		{
			TrackSPItemFields = SPItemFieldFactory.GetChangedFieldsList(this, this.ERConf.ItemAddedTrackFields);
			TrackSingleMailSPItemFields = SPItemFieldFactory.GetChangedFieldsList(this, this.ERConf.TrackFieldsSingleMail);
		}

		public override void SetEventArgs()
		{
			SetEventArgsTrackFields(ERConf.MailTemplates["_listMode"]["MAIL_SUBJECT_ITEMS_ADDED"], ERConf.MailTemplates["_listMode"]["MAIL_CREATED_BY_TEMPLATE"]);
			SetEventArgsTrackFieldsSingleMail();
		}
	}
}
