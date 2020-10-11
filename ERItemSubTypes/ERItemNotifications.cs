using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    abstract class ERItemNotifications<T> : ERItem<T> where T: ListsNotifications.ERConfNotifications
	{
        public List<SPItemField> TrackSPItemFields;
        public Dictionary<SPItemField, string> TrackSingleMailSPItemFields;
		public ERItemNotifications(SPItemEventProperties properties) : base(properties)
		{ }
		abstract public void SendNotifications();

		abstract public void SetSPItemFieldsAttributesByERType();

		protected void NotificationsTrackFields()
		{
			if (TrackSPItemFields.Count == 0)
			{
				return;
			}
			MailItem mailToNotify = new MailItem(this as ERItem<ListsNotifications.ERConfNotifications>, TrackSPItemFields, "", eventType.Contains("ing"));

			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}

		protected void NotificationsSingleField()
		{
			foreach (KeyValuePair<SPItemField, string> trackField in TrackSingleMailSPItemFields)
			{
				MailItem mailToNotifySingleField = new MailItem(this as ERItem<ListsNotifications.ERConfNotifications>, new List<SPItemField> { trackField.Key }, trackField.Value, false);
				mailToNotifySingleField.SendMail(listItem.ParentList.ParentWeb);
			}
		}

		protected void NotificationsAttachments()
		{
			MailItem mailToNotify = new MailItem(this as ERItem<ListsNotifications.ERConfNotifications>);
			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}
	}
}
