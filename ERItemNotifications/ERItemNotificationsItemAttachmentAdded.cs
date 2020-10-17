using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications.ReadConfig
{
    class ERItemNotificationsItemAttachmentAdded : ERItemNotifications
	{
		public ERItemNotificationsItemAttachmentAdded(SPItemEventProperties properties) : base(properties)
		{
			SetSPItemFieldsAttributesByERType();
		}
		public override void SetSPItemFieldsAttributesByERType()
		{

		}

		public override void SendNotifications()
		{
			NotificationsAttachments(CommonConfigNotif.MAIL_SUBJECT_ATTACHMENTS);
		}
	}
}
