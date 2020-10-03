using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications.ReadConfig
{
    class ERTypeItemAttachmentAdded : ERItemNotifHandler
	{
		public ERTypeItemAttachmentAdded(SPItemEventProperties properties) : base(properties)
		{
			SetSPItemFieldsAttributesByERType();
		}
		public override void SetSPItemFieldsAttributesByERType()
		{

		}

		public override void SendNotifications()
		{
			NotificationsAttachments();
		}
	}
}
