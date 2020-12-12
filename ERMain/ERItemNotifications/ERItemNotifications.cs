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
    /// <summary>
	/// Gets all needed variables for creating email and send notifications
	/// </summary>
	abstract class ERItemNotifications : ERItem<ERConfNotifications>
	{
        public List<SPItemField> TrackSPItemFields;
        public List<SPItemField> TrackSingleMailSPItemFields;
		public readonly bool NotifiersPresent;
		public List<string> ToMails;

		public ERItemNotifications(SPItemEventProperties properties) : base(properties, CommonConfigNotif.LIST_PROPERTY_JSON_CONF)
		{
			NotifiersPresent = ERConf.to.Count > 0 || ERConf.cc.Count > 0 || ERConf.bcc.Count > 0 || ERConf.toManagers.Count > 0;

			ToMails = GetToMails();

			GetMailTemplatesConf();
		}

		private void GetMailTemplatesConf()
		{
			if (!ERConf.MailTemplates.ContainsKey("_listMode"))
			{
				ERConf.MailTemplates["_listMode"] = CommonConfigNotif.MAIL_TEMPLATES_DEFAULT;
			}

			ERConf.TrackFieldsSingleMail.ForEach(k =>
				{
					if (!ERConf.MailTemplates.ContainsKey(k))
					{
						ERConf.MailTemplates[k] = CommonConfigNotif.MAIL_TEMPLATES_DEFAULT;
					}
				}
			);
			
		}

		private List<string> GetToMails()
		{
			List<string> mails = new List<string> { };
			List<SPPrincipal> fieldsPrincipalsManagers = new List<SPPrincipal> { };

			List <SPPrincipal> fieldsPrincipals = this.GetUsersFromUsersFieldsAfter(ERConf.to);
			List<string> fieldsPrincipalsMails = SPCommon.GetUserMails(fieldsPrincipals);

			List<SPPrincipal> ManagersFieldsUsers = this.GetUsersFromUsersFieldsAfter(this.ERConf.toManagers);
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
			MailItem mailToNotify = new MailItem(
				this, 
				TrackSPItemFields, 
				subject, 
				modifiedByTemplate,
				ERConf.MailTemplates["_listMode"],
				eventType.Contains("ing")
			);

			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}

		protected void NotificationsTrackFieldsSingleMail()
		{
			if (!NotifiersPresent)
			{
				return;
			}

			foreach (SPItemField trackField in TrackSingleMailSPItemFields)
			{
				MailItem mailToNotifySingleField = new MailItem(
					this, 
					new List<SPItemField> { trackField }, 
					ERConf.MailTemplates[trackField.FieldTitle]["MAIL_SUBJECT_ITEMS"],
					ERConf.MailTemplates[trackField.FieldTitle]["MAIL_MODIFIED_BY_TEMPLATE"],
					ERConf.MailTemplates[trackField.FieldTitle],
					false
				);
				mailToNotifySingleField.SendMail(listItem.ParentList.ParentWeb);
			}
		}

		protected void NotificationsAttachments(string subject, string modifiedByTemplate)
		{
			if (!NotifiersPresent)
			{
				return;
			}

			MailItem mailToNotify = new MailItem(
				this, 
				subject, 
				modifiedByTemplate, 
				ERConf.MailTemplates["_listMode"]
			);
			mailToNotify.SendMail(listItem.ParentList.ParentWeb);
		}
	}
}
