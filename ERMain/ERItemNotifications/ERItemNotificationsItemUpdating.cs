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
			TrackSPItemFields = this.ERConf.ItemUpdatingTrackFields
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f))
				.Where(t => t.IsChanged)
				.ToList();

            TrackSingleMailSPItemFields = this.ERConf.ItemUpdatingTrackFieldsSingleMail
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f))
                .Where(t => t.IsChanged)
				.ToList();
                //.ToDictionary(t => t, t => this.ERConf.ItemUpdatingTrackFieldsSingleMail[t.fieldTitle]);

			if (TrackSPItemFields.Count == 0 && TrackSingleMailSPItemFields.Count == 0)
			{
				return;
			}

			var updatingFixedFields = this.ERConf.ItemUpdatingFixedFields
				//.AsParallel()
				.Select(f => {
					var itemField = SPItemFieldFactory.create(this, f);

					if (String.IsNullOrEmpty(itemField.friendlyFieldValueAfter))
					{
						itemField.GetFriendlyFieldValues(itemField.fieldValueAfter, out itemField.friendlyFieldValueAfter);
					};
					itemField.friendlyFieldValueBefore = "";

					return itemField;
				})
				.Where(f => !String.IsNullOrEmpty(f.friendlyFieldValueAfter))
				.ToList();

			TrackSPItemFields.AddRange(updatingFixedFields);

		}

		public override void SendNotifications()
		{
			NotificationsTrackFields(ERConf.MailTemplates["_listMode"]["MAIL_SUBJECT_ITEMS"], ERConf.MailTemplates["_listMode"]["MAIL_MODIFIED_BY_TEMPLATE"]);
			NotificationsTrackFieldsSingleMail();
			//NotificationsTrackFieldsSingleMail(ERConf.MailTemplates["_listMode"]["MAIL_MODIFIED_BY_TEMPLATE"]);
		}
	}
}
