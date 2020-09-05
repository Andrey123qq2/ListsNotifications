using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class NotifCommonConfig
    {
		public static string CODE_FIELD_NAME = "Підрозділ";
		public static string CODE_FIELD_SUFFIX = "_сотрудники";

		public static string LIST_PROPERTY_USER_FIELDS = "er_notif_user_fields";
		public static string LIST_PROPERTY_TRACK_FIELDS = "er_notif_track_fields";
		public static string LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL = "er_notif_track_fields_singlemail";
		public static string LIST_PROPERTY_MAIL_BCC = "er_notif_mailbcc";
		public static string LIST_PROPERTY_MAIL_CC = "er_notif_mailcc";

		public static string MAIL_URL_TEMPLATE = "<p>Элемент: <a href='{0}'>{1}</a></p>";
		public static string MAIL_MODIFIED_BY_TEMPLATE = "<p>Кем изменено: {0}</p>";

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
	}
}
