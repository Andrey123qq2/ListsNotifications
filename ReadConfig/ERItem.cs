using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;

namespace ListsNotifications
{
	public class ERItem : ERItemSPProperties
    {
        public List<string> TrackFields;
		public List<string> UserNotifyFields;
		public List<string> UserNotifyFieldsMails;
		public Dictionary<string, string> TrackFieldsSingleMail;
		public List<string> mailbcc;
		public List<string> mailcc;
		

		public readonly string CODE_FIELD_NAME = "Підрозділ";
        public readonly string CODE_FIELD_SUFFIX = "_сотрудники";

		public readonly List<string> USER_NOTIFY_FIELDS = new List<string> { };
		public readonly List<string> TRACK_FIELDS = new List<string> { };
		public readonly List<string> TRACK_FIELDS_SINGLEMAIL = new List<string> { };
		public readonly List<string> MAIL_BCC = new List<string> { };
		public readonly List<string> MAIL_CC = new List<string> { };

		public readonly string LIST_PROPERTY_USER_FIELDS = "er_notif_user_fields";
		public readonly string LIST_PROPERTY_TRACK_FIELDS = "er_notif_track_fields";
		public readonly string LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL = "er_notif_track_fields_singlemail";
		public readonly string LIST_PROPERTY_MAIL_BCC = "er_notif_mailbcc";
		public readonly string LIST_PROPERTY_MAIL_CC = "er_notif_mailcc";

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

		public ERItem(SPItemEventProperties properties): base(properties)
		{
			SetAttributes(listItem.ParentList);

			List<SPPrincipal> principals = this.GetUsersFromUsersFields(UserNotifyFields);
			UserNotifyFieldsMails = SPCommon.GetUserMails(principals);
		}

		public ERItem(SPList listSP)
		{
			SetAttributes(listSP);
		}

		private void SetAttributes(SPList listSP)
		{
			this.SetAttribute(listSP, out TrackFields, TRACK_FIELDS, LIST_PROPERTY_TRACK_FIELDS);
			this.SetAttribute(listSP, out UserNotifyFields, USER_NOTIFY_FIELDS, LIST_PROPERTY_USER_FIELDS);
			this.SetAttribute(listSP, out mailcc, MAIL_CC, LIST_PROPERTY_MAIL_CC);
			this.SetAttribute(listSP, out mailbcc, MAIL_BCC, LIST_PROPERTY_MAIL_BCC);
			this.SetAttribute(listSP, out TrackFieldsSingleMail, LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL);
		}
	}
}
