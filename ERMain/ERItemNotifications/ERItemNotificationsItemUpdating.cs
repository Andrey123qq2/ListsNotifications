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
		}
		public override void SetSPItemFieldsByERType()
		{
			TrackSPItemFields = SPItemFieldFactory.GetChangedFieldsList(this, this.ERConf.ItemUpdatingTrackFields);
            TrackSingleMailSPItemFields = SPItemFieldFactory.GetChangedFieldsList(this, this.ERConf.TrackFieldsSingleMail);

			if (TrackSPItemFields.Count == 0 && TrackSingleMailSPItemFields.Count == 0)
				return;

			var updatingFixedFields = this.ERConf.ItemUpdatingFixedFields
				//.AsParallel()
				.Select(f => {
					var itemField = SPItemFieldFactory.Create(this, f);
					if (String.IsNullOrEmpty(itemField.FriendlyFieldValueAfter))
						itemField.GetFriendlyFieldValues(itemField.FieldValueAfter.ToString(), out itemField.FriendlyFieldValueAfter);
					itemField.FriendlyFieldValueBefore = "";
					return itemField;
				})
				.Where(f => !String.IsNullOrEmpty(f.FriendlyFieldValueAfter))
				.ToList();

			TrackSPItemFields.AddRange(updatingFixedFields);
		}

		public override void SetEventArgs()
		{
			SetEventArgsTrackFields(
				ERConf.MailTemplates["_listMode"]["MAIL_SUBJECT_ITEMS"], 
				ERConf.MailTemplates["_listMode"]["MAIL_MODIFIED_BY_TEMPLATE"]
			);
			SetEventArgsTrackFieldsSingleMail();
		}
	}
}
