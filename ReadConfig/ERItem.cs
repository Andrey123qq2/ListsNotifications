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
using System.Linq;

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

		public List<SPItemField> TrackSPItemFields;
		public Dictionary<SPItemField, string> TrackSingleMailSPItemFields;
		//public List<SPItemField> TrackFieldsSingleMailSPItemField;

		public ERItem(SPItemEventProperties properties): base(properties)
		{
			if (listItem == null)
			{
				return;
			};

			SetAllAttributes(listItem.ParentList);

			List<SPPrincipal> principals = this.GetUsersFromUsersFields(UserNotifyFields);
			UserNotifyFieldsMails = SPCommon.GetUserMails(principals);

			TrackSPItemFields = TrackFields
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f))
				.Where(t => t.IsChanged)
				.ToList();

			TrackSingleMailSPItemFields = TrackFieldsSingleMail
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f.Key))
				.Where(t => t.IsChanged)
				.ToDictionary(t => t, t => TrackFieldsSingleMail[t.fieldTitle]);
		}

		public ERItem(SPList listSP)
		{
			SetAllAttributes(listSP);
		}

		private void SetAllAttributes(SPList listSP)
		{
			this.SetAttribute(listSP, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);
			this.SetAttribute(listSP, out UserNotifyFields, NotifCommonConfig.LIST_PROPERTY_USER_FIELDS, true);
			this.SetAttribute(listSP, out mailcc, NotifCommonConfig.LIST_PROPERTY_MAIL_CC);
			this.SetAttribute(listSP, out mailbcc, NotifCommonConfig.LIST_PROPERTY_MAIL_BCC);
			this.SetAttribute(listSP, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);
		}

		//private void GetSPItemFieldsChanges()
		//{
		//	TrackSPItemField = TrackFields.Select(f => SPItemFieldFactory.create(this, f)).Where(t => t.IsChanged).ToList();
		//}
	}
}
