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
        public readonly string body;
        public readonly string to;
        public readonly string cc;
        public readonly string bcc;
        public readonly string subject;
        StringDictionary headers;

        private bool showBeforeValues;
        private string fieldStringTemplate;
        private bool eventTypeAttachment;

        public MailItem(ERItem item, List<SPItemField> fieldsToTrack, string mailSubjectMode = "", bool showBeforeValuesParam = true)
        {
            showBeforeValues = showBeforeValuesParam;
            fieldStringTemplate = GetFieldStringTemplate();
            eventTypeAttachment = item.eventProperties.EventType.ToString().Contains("Attachment");

            body = CreateBody(item, fieldsToTrack);
            to = String.Join(",", item.UserNotifyFieldsMails);
            cc = String.Join(",", item.mailcc);
            bcc = String.Join(",", item.mailbcc);
            subject = CreateSubject(item, mailSubjectMode);
            headers = GetHeaders();
        }

        private string GetFieldStringTemplate()
        {
            string stringTemplate;
            if (showBeforeValues)
            {
                stringTemplate = "<p>{0}: <strike>{1}</strike> {2}</p>";
            }
            else
            {
                stringTemplate = "<p>{0}: {2}</p>";
            }

            if (eventTypeAttachment)
            {
                stringTemplate = "<p>{0}: <a href=\"{1}\">{2}</a></p>";
            }

            return stringTemplate;
        }

        private string CreateSubject(ERItem item, string subjectMode)
        {
            string mailSubject;

            if (subjectMode == "")
            {
                if (!item.eventProperties.EventType.ToString().Contains("Attachment"))
                {
                    subjectMode = "элемент изменен";
                }
                else
                {
                    subjectMode = "добавлено вложение";
                }
            }

            mailSubject = String.Format("{0}: {1}", item.itemTitle, subjectMode);

            return mailSubject;
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

        private string GetChangedFieldsBlock(ERItem item)
        {
            string ChangedFieldsBlock = "";

            string attachmentUrl = item.listItem.Web.Url + "/" + item.eventProperties.AfterUrl.ToString();
            string attachmentName = Regex.Replace(attachmentUrl, @"^.*\/", "");
            ChangedFieldsBlock += String.Format(fieldStringTemplate, "Вложение", attachmentUrl, attachmentName);

            return ChangedFieldsBlock;
        }

        private string GetChangedFieldsBlock(ERItem item, List<SPItemField> itemFields)
        {
            string ChangedFieldsBlock = "";

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

        //private string GetChangedFieldsBlock(ERItem item, List<SPItemField> itemFields)
        //{
        //    string ChangedFieldsBlock = "";

        //    if (item.eventProperties.EventType.ToString().Contains("Attachment"))
        //    {
        //        ChangedFieldsBlock = GetChangedFieldsBlockAttachment(item, itemFields);
        //    }
        //    else
        //    {
        //        ChangedFieldsBlock = GetChangedFieldsBlockNotAttachment(item, itemFields);
        //    }


        //    return ChangedFieldsBlock;
        //}

            //string FieldStringTemplate;

            //if (!item.eventProperties.EventType.ToString().Contains("Attachment"))
            //{
            //    foreach (SPItemField field in itemFields)
            //    {
            //        //if (!item.listItem.ParentList.Fields.ContainsField(fieldTitle))
            //        //{
            //        //    continue;
            //        //}

            //        //if (item.FieldIsChanged(fieldTitle))
            //        //{
            //        //    string beforeFieldValue = item.GetFriendlyFieldValue(fieldTitle, false);
            //        //    string afterFieldValue = item.GetFriendlyFieldValue(fieldTitle);

            //        //    if (!BeforeValues && (afterFieldValue == "-" || afterFieldValue == ""))
            //        //    {
            //        //        continue;
            //        //    }

            //        //    if (BeforeValues)
            //        //    {
            //        //        FieldStringTemplate = "<p>{0}: <strike>{1}</strike> {2}</p>";
            //        //    }
            //        //    else 
            //        //    {
            //        //        FieldStringTemplate = "<p>{0}: {2}</p>";
            //        //    }
            //        //    ChangedFieldsBlock += String.Format(FieldStringTemplate, fieldTitle, beforeFieldValue, afterFieldValue);
            //        //}
            //    }
            //}
            //else
            //{
            //    string attachmentUrl = item.listItem.Web.Url + "/" + item.eventProperties.AfterUrl.ToString();
            //    string attachmentName = Regex.Replace(attachmentUrl, @"^.*\/", "");
            //    ChangedFieldsBlock += String.Format("<p>{0}: <a href=\"{1}\">{2}</a></p>", "Вложение", attachmentUrl, attachmentName);
            //}

            //return ChangedFieldsBlock;
        //}

        private string CreateBody(ERItem item, List<SPItemField> fields)
        {
            string ChangedFieldsBlock;
            string ModifiedByBlock;
            string itemUrlBlock;
            string EditorDisplayName;

            string mailBodyString;

            ChangedFieldsBlock = eventTypeAttachment ? GetChangedFieldsBlock(item, fields) : GetChangedFieldsBlock(item);

            if (ChangedFieldsBlock == "")
            {
                return "";
            }

            itemUrlBlock = String.Format(NotifCommonConfig.MAIL_URL_TEMPLATE, item.listItem.GetItemFullUrl(), item.itemTitle);
            EditorDisplayName = item.eventProperties.UserDisplayName;
            ModifiedByBlock = String.Format(NotifCommonConfig.MAIL_MODIFIED_BY_TEMPLATE, EditorDisplayName);

            mailBodyString = String.Format(NotifCommonConfig.MAIL_BODY_TEMPLATE, ChangedFieldsBlock, itemUrlBlock, ModifiedByBlock);

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