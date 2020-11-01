using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPERCommonLib;

namespace ListsNotifications
{
    abstract class ERItemNotifications : ERItem<ERConfNotifications>
	{
        public List<SPItemField> TrackSPItemFields;
        public Dictionary<SPItemField, string> TrackSingleMailSPItemFields;
		public readonly bool NotifiersPresent;
		public List<string> toMails;

		public ERItemNotifications(SPItemEventProperties properties) : base(properties, CommonConfigNotif.LIST_PROPERTY_JSON_CONF)
		{
			NotifiersPresent = ERConf.to.Count > 0 || ERConf.cc.Count > 0 || ERConf.bcc.Count > 0 || ERConf.toManagers.Count > 0;

			toMails = GetToMails();
		}

		private List<string> GetToMails()
		{
			List<string> mails = new List<string> { };
			List<SPPrincipal> fieldsPrincipalsManagers = new List<SPPrincipal> { };

			List <SPPrincipal> fieldsPrincipals = this.listItem.GetUsersFromUsersFields(ERConf.to);
			List<string> fieldsPrincipalsMails = SPCommon.GetUserMails(fieldsPrincipals);

			List<SPPrincipal> ManagersFieldsUsers = this.listItem.GetUsersFromUsersFields(this.ERConf.toManagers);
			List<List<SPPrincipal>> ManagersFieldsManagers = ManagersFieldsUsers
				.Where(p => p.GetType().Name == "SPUser")
				.Select(u => ((SPUser)u).GetUserManagers()).ToList<List<SPPrincipal>>();
			ManagersFieldsManagers.ForEach(lp => { fieldsPrincipalsManagers.AddRange(lp); });
			List<string> fieldsPrincipalsManagersMails = SPCommon.GetUserMails(fieldsPrincipalsManagers);

			mails.AddRange(fieldsPrincipalsMails);
			mails.AddRange(fieldsPrincipalsManagersMails);

			return mails;
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

		protected void NotificationsAttachments(string subject, string modifiedByTemplate)
		{
			if (!NotifiersPresent)
			{
				return;
			}

			MailItem mailToNotify = new MailItem(this, subject, modifiedByTemplate);
			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}
	}
}
