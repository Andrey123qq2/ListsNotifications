using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    abstract class ERItemNotifications : ERItem<ERConfNotifications> //where T: ListsNotifications.ERConfNotifications
	{
        public List<SPItemField> TrackSPItemFields;
        public Dictionary<SPItemField, string> TrackSingleMailSPItemFields;
		public readonly bool NotifiersPresent;
		public List<string> toMails;

		public ERItemNotifications(SPItemEventProperties properties) : base(properties)
		{
			NotifiersPresent = ERConf.to.Count > 0 || ERConf.cc.Count > 0 || ERConf.bcc.Count > 0;

			List<SPPrincipal> principals = this.GetUsersFromUsersFields(ERConf.to);
			toMails = SPCommon.GetUserMails(principals);
		}
		abstract public void SendNotifications();

		abstract public void SetSPItemFieldsAttributesByERType();

		protected void NotificationsTrackFields()
		{
			if (TrackSPItemFields.Count == 0)
			{
				return;
			}
			MailItem mailToNotify = new MailItem(this, TrackSPItemFields, "", eventType.Contains("ing"));

			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}

		protected void NotificationsSingleField()
		{
			foreach (KeyValuePair<SPItemField, string> trackField in TrackSingleMailSPItemFields)
			{
				MailItem mailToNotifySingleField = new MailItem(this, new List<SPItemField> { trackField.Key }, trackField.Value, false);
				mailToNotifySingleField.SendMail(listItem.ParentList.ParentWeb);
			}
		}

		protected void NotificationsAttachments()
		{
			MailItem mailToNotify = new MailItem(this);
			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}
	}
}
