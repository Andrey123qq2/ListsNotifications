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

            TrackSingleMailSPItemFields = this.ERConf.TrackFieldsSingleMail
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f))
                .Where(t => t.IsChanged)
				.ToList();

			if (TrackSPItemFields.Count == 0 && TrackSingleMailSPItemFields.Count == 0)
			{
				return;
			}

			var updatingFixedFields = this.ERConf.ItemUpdatingFixedFields
				//.AsParallel()
				.Select(f => {
					var itemField = SPItemFieldFactory.create(this, f);

					if (String.IsNullOrEmpty(itemField.FriendlyFieldValueAfter))
					{
						itemField.GetFriendlyFieldValues(itemField.FieldValueAfter, out itemField.FriendlyFieldValueAfter);
					};
					itemField.FriendlyFieldValueBefore = "";

					return itemField;
				})
				.Where(f => !String.IsNullOrEmpty(f.FriendlyFieldValueAfter))
				.ToList();

			TrackSPItemFields.AddRange(updatingFixedFields);

		}

		public override void SendNotifications()
		{
			NotificationsTrackFields(ERConf.MailTemplates["_listMode"]["MAIL_SUBJECT_ITEMS"], ERConf.MailTemplates["_listMode"]["MAIL_MODIFIED_BY_TEMPLATE"]);
			NotificationsTrackFieldsSingleMail();
		}
	}
}
