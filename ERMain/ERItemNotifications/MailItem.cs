using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;
using SPSCommon.SPCustomExtensions;

namespace ListsNotifications
{
    /// <summary>
    /// Creates mail elements for ERItemNotifications and send mail by public method
    /// </summary>
    class MailItem
    {
        private readonly string Body;
        private readonly string Subject;
        private readonly StringDictionary Headers;
        private string To;
        private string Cc;
        private string Bcc;

        private readonly string AttachmentUrl;
        private readonly bool ShowBeforeValues;
        private readonly string ModifiedByBlockTemplate;
        private string ItemUrlBlock;
        private string EditorDisplayName;
        private string ModifiedByBlock;
        private readonly Dictionary<string, string> TemplatesParams;

        public MailItem(
            ERItemNotifications item, 
            List<SPItemField> fieldsToTrack, 
            string mailSubjectMode,
            string modifiedByBlockTemplate,
            Dictionary<string, string> templatesParams,
            bool showBeforeValuesParam = true
        )
        {
            ShowBeforeValues = showBeforeValuesParam;
            ModifiedByBlockTemplate = modifiedByBlockTemplate;
            TemplatesParams = templatesParams;

            InitCommonAttributes(item);

            Body = CreateBody(fieldsToTrack);
            Subject = String.Format("{0}: {1}", item.itemTitle, mailSubjectMode); 
            Headers = GetHeaders();
        }

        public MailItem(
            ERItemNotifications item, 
            string mailSubjectMode, 
            string modifiedByBlockTemplate, 
            Dictionary<string, string> templatesParams
        )
        {
            ModifiedByBlockTemplate = modifiedByBlockTemplate;
            AttachmentUrl = item.listItem.Web.Url + "/" + item.eventProperties.AfterUrl.ToString();
            TemplatesParams = templatesParams;

            InitCommonAttributes(item);

            Body = CreateBody();
            Subject = String.Format("{0}: {1}", item.itemTitle, mailSubjectMode);
            Headers = GetHeaders();
        }

        private void InitCommonAttributes(ERItemNotifications item)
        {
            To = String.Join(",", item.ToMails);
            Cc = String.Join(",", item.ERConf.cc);
            Bcc = String.Join(",", item.ERConf.bcc);

            //itemAdded = item.eventType.Contains("Added");

            ItemUrlBlock = String.Format(TemplatesParams["MAIL_URL_TEMPLATE"], item.listItem.GetItemFullUrl(), item.itemTitle);
            EditorDisplayName = item.eventProperties.UserDisplayName;
            //string ModifiedByBlockTemplate = itemAdded ? CommonConfigNotif.MAIL_CREATED_BY_TEMPLATE : CommonConfigNotif.MAIL_MODIFIED_BY_TEMPLATE;
            ModifiedByBlock = String.Format(ModifiedByBlockTemplate, EditorDisplayName);
        }
        private StringDictionary GetHeaders()
        {
            StringDictionary mailHeaders = new StringDictionary();
            mailHeaders.Add("to", To);
            mailHeaders.Add("cc", Cc);
            mailHeaders.Add("bcc", Bcc);
            mailHeaders.Add("subject", Subject);
            mailHeaders.Add("content-type", "text/html");
            return mailHeaders;
        }
        private string GetChangedFieldsBlock(List<SPItemField> itemFields)
        {
            string ChangedFieldsBlock = "";
            string fieldStringTemplate = ShowBeforeValues ? TemplatesParams["MAIL_FIELDS_TEMPLATE_ITEMS_BEFORE"] : TemplatesParams["MAIL_FIELDS_TEMPLATE_ITEMS_NOTBEFORE"];

            foreach (SPItemField field in itemFields)
            {
                // TODO: Move condition block to another/field class
                if (!ShowBeforeValues && (field.FriendlyFieldValueAfter == "-" || field.FriendlyFieldValueAfter == ""))
                {
                    continue;
                }

                ChangedFieldsBlock += String.Format(fieldStringTemplate, field.FieldTitle, field.FriendlyFieldValueBefore, field.FriendlyFieldValueAfter);
            }

            return ChangedFieldsBlock;
        }
        private string GetChangedFieldsBlock()
        {
            string attachmentName = Regex.Replace(AttachmentUrl, @"^.*\/", "");
            string ChangedFieldsBlock = String.Format(TemplatesParams["MAIL_FIELDS_TEMPLATE_ATTACHMENTS"], TemplatesParams["MAIL_BODY_ATTACHMENTS"], AttachmentUrl, attachmentName);

            return ChangedFieldsBlock;
        }
        private string CreateBody(List<SPItemField> fields)
        {
            string ChangedFieldsBlock;
            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock(fields);

            mailBodyString = ChangedFieldsBlock != "" ? String.Format(TemplatesParams["MAIL_BODY_TEMPLATE"], ChangedFieldsBlock, ItemUrlBlock, ModifiedByBlock) : "";

            return mailBodyString;
        }

        private string CreateBody()
        {
            string ChangedFieldsBlock;
            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock();
            mailBodyString = String.Format(TemplatesParams["MAIL_BODY_TEMPLATE"], ChangedFieldsBlock, ItemUrlBlock, ModifiedByBlock);

            return mailBodyString;
        }

        public void SendMail(SPWeb web)
        {
            if (Body != "" && (To != "" || Cc != "" || Bcc != ""))
            {
                SPUtility.SendEmail(web, Headers, Body);
            }
        }
    }
}