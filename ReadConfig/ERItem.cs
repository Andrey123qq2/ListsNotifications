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
using System.Reflection;

namespace ListsNotifications
{
	public class ERItem : ERItemSPProperties
    {
		public List<string> UserNotifyFields;
		public List<string> UserNotifyFieldsMails;
		public List<string> mailbcc;
		public List<string> mailcc;
		public bool NotifiersPresent = true;

		public List<string> TrackFields;
		public Dictionary<string, string> TrackFieldsSingleMail;
		public List<string> ItemCreateFields;

		public List<SPItemField> TrackSPItemFields;
		public Dictionary<SPItemField, string> TrackSingleMailSPItemFields;

		public ERItem(SPItemEventProperties properties) : base(properties)
		{
			SetMailAttributes(listItem.ParentList);

			if (UserNotifyFields.Count == 0 && mailcc.Count == 0 && mailbcc.Count == 0)
			{
				NotifiersPresent = false;

				return;
			}

			NotifiersPresent = true;

			List<SPPrincipal> principals = this.GetUsersFromUsersFields(UserNotifyFields);
			UserNotifyFieldsMails = SPCommon.GetUserMails(principals);

			SetSPItemFieldsAttributesByERType();
		}

		public ERItem(SPList listSP)
		{
			SetMailAttributes(listSP);

			this.SetAttribute(listSP, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);
			this.SetAttribute(listSP, out ItemCreateFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMCREATE, true);
			this.SetAttribute(listSP, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);
		}

		private void SetMailAttributes(SPList listSP)
		{
			this.SetAttribute(listSP, out UserNotifyFields, NotifCommonConfig.LIST_PROPERTY_USER_FIELDS, true);
			this.SetAttribute(listSP, out mailcc, NotifCommonConfig.LIST_PROPERTY_MAIL_CC);
			this.SetAttribute(listSP, out mailbcc, NotifCommonConfig.LIST_PROPERTY_MAIL_BCC);
		}

		private void SetSPItemFieldsAttributesByERType()
		{
			string SetAttributesMethodName = "SetAttributes" + eventType;

			Type calledType = typeof(ERItem);
			calledType.InvokeMember(
				SetAttributesMethodName,
				BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
				null,
				null,
				null
			);
		}

		private void SetAttributesItemUpdating()
		{
			this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);

			TrackSPItemFields = TrackFields
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f))
				.Where(t => t.IsChanged)
				.ToList();
		}
		private void SetAttributesItemUpdated()
		{
			this.SetAttribute(listItem.ParentList, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);

			TrackSingleMailSPItemFields = TrackFieldsSingleMail
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f.Key))
				.Where(t => t.IsChanged)
				.ToDictionary(t => t, t => TrackFieldsSingleMail[t.fieldTitle]);
		}
		private void SetAttributesItemAdded()
		{
			this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMCREATE, true);

			TrackSPItemFields = TrackFields
				//.AsParallel()
				.Select(f => SPItemFieldFactory.create(this, f, false))
				.Where(t => t.IsChanged)
				.ToList();
		}
		private void SetAttributesAttachmentAdding(SPList listSP)
		{

		}
	}
}
