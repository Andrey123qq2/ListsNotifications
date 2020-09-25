using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
	static class NotifCommonConfig
    {
		public static readonly string CODE_FIELD_NAME = "Підрозділ";
		public static readonly string CODE_FIELD_SUFFIX = "_сотрудники";

		public static readonly string LIST_PROPERTY_USER_FIELDS = "er_notif_user_fields";

		public static readonly string LIST_PROPERTY_TRACK_FIELDS = "er_notif_track_fields";
		public static readonly string LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL = "er_notif_track_fields_singlemail";
		public static readonly string LIST_PROPERTY_TRACK_FIELDS_ITEMADDED = "er_notif_track_fields_itemcreate";

		public static Dictionary<string, string> LIST_PROPERTY_TRACK_FIELDS_BY_ERTYPE = new Dictionary<string, string>()
		{
			["ItemUpdating"] = LIST_PROPERTY_TRACK_FIELDS,
			["ItemUpdated"] = LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL,
			["ItemAdded"] = LIST_PROPERTY_TRACK_FIELDS_ITEMADDED,
			["ItemAttachmentAdded"] = ""
		};


		public static readonly string LIST_PROPERTY_MAIL_BCC = "er_notif_mailbcc";
		public static readonly string LIST_PROPERTY_MAIL_CC = "er_notif_mailcc";

		public static readonly string MAIL_URL_TEMPLATE = "<p>Элемент: <a href='{0}'>{1}</a></p>";
		public static readonly string MAIL_MODIFIED_BY_TEMPLATE = "<p>Кем изменено: {0}</p>";
		public static readonly string MAIL_SUBJECT_ATTACHMENTS = "добавлено вложение";
		public static readonly string MAIL_SUBJECT_ITEMS = "элемент изменен";
		public static readonly string MAIL_FIELDS_TEMPLATE_ATTACHMENTS = "<p>{0}: <a href=\"{1}\">{2}</a></p>";
		public static readonly string MAIL_FIELDS_TEMPLATE_ITEMS_BEFORE = "<p>{0}: <strike>{1}</strike> {2}</p>";
		public static readonly string MAIL_FIELDS_TEMPLATE_ITEMS_NOTBEFORE = "<p>{0}: {2}</p>";
		public static readonly string MAIL_BODY_ATTACHMENTS = "Вложение";

		public static readonly string MAIL_BODY_TEMPLATE = @"
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
	}
}
