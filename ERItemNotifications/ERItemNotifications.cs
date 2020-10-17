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

			List<SPPrincipal> principals = this.listItem.GetUsersFromUsersFields(ERConf.to);
			toMails = SPCommon.GetUserMails(principals);
		}
		abstract public void SendNotifications();

		abstract public void SetSPItemFieldsAttributesByERType();

		protected void NotificationsTrackFields(string subject, string modifiedByTemplate)
		{
			if (TrackSPItemFields.Count == 0 || !NotifiersPresent)
			{
				return;
			}
			MailItem mailToNotify = new MailItem(this, TrackSPItemFields, subject, modifiedByTemplate, eventType.Contains("ing"));

			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}

		protected void NotificationsTrackFieldsSingleMail(string modifiedByTemplate)
		{
			if (!NotifiersPresent)
			{
				return;
			}

			foreach (KeyValuePair<SPItemField, string> trackField in TrackSingleMailSPItemFields)
			{
				MailItem mailToNotifySingleField = new MailItem(this, new List<SPItemField> { trackField.Key }, trackField.Value, modifiedByTemplate, false);
				mailToNotifySingleField.SendMail(listItem.ParentList.ParentWeb);
			}
		}

		protected void NotificationsAttachments(string subject)
		{
			if (!NotifiersPresent)
			{
				return;
			}

			MailItem mailToNotify = new MailItem(this, subject);
			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}
	}
}
