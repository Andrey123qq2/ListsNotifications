using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ListsNotifications
{
    class MailItem
    {
        private string body;
        private string to;
        private string cc;
        private string bcc;
        private string subject;
        StringDictionary headers;
        private string attachmentUrl;

        private bool showBeforeValues;

        string itemUrlBlock;
        string EditorDisplayName;
        string ModifiedByBlock;

        public MailItem(ERItem<ERConfNotifications> item, List<SPItemField> fieldsToTrack, string mailSubjectMode = "", bool showBeforeValuesParam = true)
        {
            showBeforeValues = showBeforeValuesParam;
            InitCommonAttributes(item);

            body = CreateBody(fieldsToTrack);
            subject = String.Format("{0}: {1}", item.itemTitle, mailSubjectMode != "" ? mailSubjectMode: CommonConfigNotif.MAIL_SUBJECT_ITEMS); 
            headers = GetHeaders();
        }

        public MailItem(ERItem<ERConfNotifications> item)
        {
            attachmentUrl = item.listItem.Web.Url + "/" + item.eventProperties.AfterUrl.ToString();

            InitCommonAttributes(item);

            body = CreateBody();
            subject = String.Format("{0}: {1}", item.itemTitle, CommonConfigNotif.MAIL_SUBJECT_ATTACHMENTS);
            headers = GetHeaders();
        }

        private void InitCommonAttributes(ERItem<ERConfNotifications> item)
        {
            to = String.Join(",", item.UserNotifyFieldsMails);
            cc = String.Join(",", item.mailcc);
            bcc = String.Join(",", item.mailbcc);

            itemUrlBlock = String.Format(CommonConfigNotif.MAIL_URL_TEMPLATE, item.listItem.GetItemFullUrl(), item.itemTitle);
            EditorDisplayName = item.eventProperties.UserDisplayName;
            ModifiedByBlock = String.Format(CommonConfigNotif.MAIL_MODIFIED_BY_TEMPLATE, EditorDisplayName);
        }
        private StringDictionary GetHeaders()
        {
            StringDictionary mailHeaders = new StringDictionary();
            mailHeaders.Add("to", to);
            mailHeaders.Add("cc", cc);
            mailHeaders.Add("bcc", bcc);
            mailHeaders.Add("subject", subject);
            mailHeaders.Add("content-type", "text/html");
            return mailHeaders;
        }
        private string GetChangedFieldsBlock(List<SPItemField> itemFields)
        {
            string ChangedFieldsBlock = "";
            string fieldStringTemplate = showBeforeValues ? CommonConfigNotif.MAIL_FIELDS_TEMPLATE_ITEMS_BEFORE : CommonConfigNotif.MAIL_FIELDS_TEMPLATE_ITEMS_NOTBEFORE;

            foreach (SPItemField field in itemFields)
            {
                if (!showBeforeValues && (field.friendlyFieldValueAfter == "-" || field.friendlyFieldValueAfter == ""))
                {
                    continue;
                }

                ChangedFieldsBlock += String.Format(fieldStringTemplate, field.fieldTitle, field.friendlyFieldValueBefore, field.friendlyFieldValueAfter);
            }

            return ChangedFieldsBlock;
        }
        private string GetChangedFieldsBlock()
        {
            string attachmentName = Regex.Replace(attachmentUrl, @"^.*\/", "");
            string ChangedFieldsBlock = String.Format(CommonConfigNotif.MAIL_FIELDS_TEMPLATE_ATTACHMENTS, CommonConfigNotif.MAIL_BODY_ATTACHMENTS, attachmentUrl, attachmentName);

            return ChangedFieldsBlock;
        }
        private string CreateBody(List<SPItemField> fields)
        {
            string ChangedFieldsBlock;
            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock(fields);

            mailBodyString = ChangedFieldsBlock != "" ? String.Format(CommonConfigNotif.MAIL_BODY_TEMPLATE, ChangedFieldsBlock, itemUrlBlock, ModifiedByBlock) : "";

            return mailBodyString;
        }

        private string CreateBody()
        {
            string ChangedFieldsBlock;
            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock();
            mailBodyString = String.Format(CommonConfigNotif.MAIL_BODY_TEMPLATE, ChangedFieldsBlock, itemUrlBlock, ModifiedByBlock);

            return mailBodyString;
        }

        public void SendMail(SPWeb web)
        {
            if (body != "" && (to != "" || cc != "" || bcc != ""))
            {
                SPUtility.SendEmail(web, headers, body);
            }
        }
    }
}