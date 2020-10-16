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
	//abstract public class ERItemBak : ERItemSPProperties
	//{
	//	public List<string> UserNotifyFields;
	//	public List<string> UserNotifyFieldsMails;
	//	public List<string> mailbcc;
	//	public List<string> mailcc;
	//	public bool NotifiersPresent = true;

	//	public List<string> TrackFields;
	//	public List<string> ItemAddedFields; // for web page settings
	//	public Dictionary<string, string> TrackFieldsSingleMail;
	//	//public List<string> ItemCreateFields;

	//	public List<SPItemField> TrackSPItemFields;
	//	public Dictionary<SPItemField, string> TrackSingleMailSPItemFields;

	//	public ERItem(SPItemEventProperties properties) : base(properties)
	//	{
	//		SetMailAttributes(listItem.ParentList);

	//		if (UserNotifyFields.Count == 0 && mailcc.Count == 0 && mailbcc.Count == 0)
	//		{
	//			NotifiersPresent = false;

	//			return;
	//		}

	//		NotifiersPresent = true;

	//		List<SPPrincipal> principals = this.GetUsersFromUsersFields(UserNotifyFields);
	//		UserNotifyFieldsMails = SPCommon.GetUserMails(principals);

	//		//SetSPItemFieldsAttributesByERType();
	//	}

	//	abstract public void SendNotifications();

	//	abstract public void SetSPItemFieldsAttributesByERType();

	//	public ERItem(SPList listSP)
	//	{
	//		SetMailAttributes(listSP);

	//		//this.SetAttribute(listSP, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);
	//		//this.SetAttribute(listSP, out ItemCreateFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMCREATE, true);
	//		//this.SetAttribute(listSP, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);
	//	}

	//	private void SetMailAttributes(SPList listSP)
	//	{
	//		this.SetAttribute(listSP, out UserNotifyFields, NotifCommonConfig.LIST_PROPERTY_USER_FIELDS, true);
	//		this.SetAttribute(listSP, out mailcc, NotifCommonConfig.LIST_PROPERTY_MAIL_CC);
	//		this.SetAttribute(listSP, out mailbcc, NotifCommonConfig.LIST_PROPERTY_MAIL_BCC);
	//	}

	//	//public static ERItem create(SPItemEventProperties properties)
	//	public static ERItem Create(params dynamic[] args)
	//	{

	//		if (args.Length != 0)
	//		{
	//			SPItemEventProperties properties = (SPItemEventProperties)args[0];
	//			object[] ERItemParams = { properties };

	//			Type ERItemType = GetERItemType(properties);

	//			return (ERItem)Activator.CreateInstance(ERItemType, ERItemParams);
	//		}
	//		else
	//		{
	//			Type ERItemType = GetERItemType();

	//			return (ERItem)Activator.CreateInstance(ERItemType);
	//		}
	//	}


	//	private static Type GetERItemType(SPItemEventProperties properties)
	//	{
	//		Type ERItemType;
	//		string ERItemTypeName;

	//		ERItemTypeName = "ERType" + properties.EventType.ToString();

	//		string assemblyName = "ListsNotifications";

	//		ERItemType = Type.GetType(assemblyName + "." + ERItemTypeName);

	//		//if (ERItemType == null)
	//		//{
	//		//	ERItemTypeName = Type.GetType(assemblyName + ".ERItemTypeCommon");
	//		//}

	//		return ERItemType;
	//	}

	//	private static Type GetERItemType()
	//	{
	//		Type ERItemType;
	//		string ERItemTypeName;

	//		ERItemTypeName = "ERTypeItemPageSettings";

	//		string assemblyName = "ListsNotifications";

	//		ERItemType = Type.GetType(assemblyName + "." + ERItemTypeName);

	//		return ERItemType;
	//	}

	//	protected void NotificationsTrackFields()
	//	{
	//		if (TrackSPItemFields.Count == 0)
	//		{
	//			return;
	//		}

	//		MailItem mailToNotify = new MailItem(this, TrackSPItemFields, "", eventType.Contains("ing"));

	//		mailToNotify.SendMail(listItem.ParentList.ParentWeb);
	//	}

	//	protected void NotificationsSingleField()
	//	{
	//		foreach (KeyValuePair<SPItemField, string> trackField in TrackSingleMailSPItemFields)
	//		{
	//			MailItem mailToNotifySingleField = new MailItem(this, new List<SPItemField> { trackField.Key }, trackField.Value, false);
	//			mailToNotifySingleField.SendMail(listItem.ParentList.ParentWeb);
	//		}
	//	}

	//	protected void NotificationsAttachments()
	//	{
	//		MailItem mailToNotify = new MailItem(this);
	//		mailToNotify.SendMail(listItem.ParentList.ParentWeb);
	//	}

	//	//private void SetSPItemFieldsAttributesByERType()
	//	//{
	//	//	string SetAttributesMethodName = "SetAttributes" + eventType;

	//	//	Type calledType = typeof(ERItem);
	//	//	calledType.InvokeMember(
	//	//		SetAttributesMethodName,
	//	//		BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
	//	//		null,
	//	//		null,
	//	//		null
	//	//	);
	//	//}

	//	//private void SetAttributesItemUpdating()
	//	//{
	//	//	this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS, true);

	//	//	TrackSPItemFields = TrackFields
	//	//		//.AsParallel()
	//	//		.Select(f => SPItemFieldFactory.create(this, f))
	//	//		.Where(t => t.IsChanged)
	//	//		.ToList();
	//	//}
	//	//private void SetAttributesItemUpdated()
	//	//{
	//	//	this.SetAttribute(listItem.ParentList, out TrackFieldsSingleMail, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL, true);

	//	//	TrackSingleMailSPItemFields = TrackFieldsSingleMail
	//	//		//.AsParallel()
	//	//		.Select(f => SPItemFieldFactory.create(this, f.Key))
	//	//		.Where(t => t.IsChanged)
	//	//		.ToDictionary(t => t, t => TrackFieldsSingleMail[t.fieldTitle]);
	//	//}
	//	//private void SetAttributesItemAdded()
	//	//{
	//	//	this.SetAttribute(listItem.ParentList, out TrackFields, NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMCREATE, true);

	//	//	TrackSPItemFields = TrackFields
	//	//		//.AsParallel()
	//	//		.Select(f => SPItemFieldFactory.create(this, f, false))
	//	//		.Where(t => t.IsChanged)
	//	//		.ToList();
	//	//}
	//	//private void SetAttributesAttachmentAdding(SPList listSP)
	//	//{

	//	//}
	//}
}
