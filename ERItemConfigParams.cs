using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;

namespace ListsNotifications
{
	public class ERItemConfigParams : ERItemConfig
    {
        public List<string> TrackFields;
        public List<string> UserNotifyFields;
		public List<string> MailBcc;

		public readonly string CODE_FIELD_NAME = "Підрозділ";
        public readonly string CODE_FIELD_SUFFIX = "_сотрудники";

        public readonly List<string> USER_NOTIFY_FIELDS = new List<string> { "Постановщик", "Author", "Заказчик", "Редакторы", "Рабочая группа" };
		public readonly List<string> TRACK_FIELDS = new List<string> { };
		public readonly List<string> MAIL_BCC = new List<string> { };
		public readonly string LIST_PROPERTY_USER_FIELDS = "er_notif_user_fields";
		public readonly string LIST_PROPERTY_TRACK_FIELDS = "er_notif_track_fields";
		public readonly string LIST_PROPERTY_MAIL_BCC = "er_notif_mail_bcc";

		public static string MAIL_BODY_TEMPLATE = @"
<!DOCTYPE html>
<html>
	<style>
		table{{
			font-family: Segoe UI Light;
			font-size: 11.5pt;
		}}
        body {{
			font-size: 13.5pt;
			font-family: Segoe UI Light,sans-serif;
			color: #444444;
		}}
		.expired {{
			color: EXPIRED_COLOR;
		}}
		#itemurl {{
			font-size: 16.5pt;
		}}
	</style>
	<body>
		<div>
			{0}
            {1}
            {2}
		</div>
	</body>
</html>
";

		//public ERItemConfigParams(SPList List)
		public void SetConfigAttributes(SPList List)
		{
			this.SetListAttribute(List, out TrackFields, TRACK_FIELDS, LIST_PROPERTY_TRACK_FIELDS);
			this.SetListAttribute(List, out UserNotifyFields, USER_NOTIFY_FIELDS, LIST_PROPERTY_USER_FIELDS);
			this.SetListAttribute(List, out MailBcc, MAIL_BCC, LIST_PROPERTY_MAIL_BCC);
		}
	}
}
