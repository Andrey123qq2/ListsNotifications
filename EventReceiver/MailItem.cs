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
        private bool BeforeValues;

        public MailItem(ERItem item, List<string> fieldsToTrack, string mailSubjectMode = "", bool FieldsBeforeValue = true)
        {
            BeforeValues = FieldsBeforeValue;
            body = CreateBody(item, fieldsToTrack);
            to = String.Join(",", item.UserNotifyFieldsMails);
            cc = String.Join(",", item.mailcc);
            bcc = String.Join(",", item.mailbcc);
            subject = CreateSubject(item, mailSubjectMode);
            headers = GetHeaders();
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
            //mailHeaders.Add("cc", cc);
            mailHeaders.Add("bcc", bcc);
            mailHeaders.Add("subject", subject);
            mailHeaders.Add("content-type", "text/html");
            return mailHeaders;
        }

        private string GetChangedFieldsBlock(ERItem item, List<string> fields)
        {
            string ChangedFieldsBlock = "";
            string FieldStringTemplate;

            if (!item.eventProperties.EventType.ToString().Contains("Attachment"))
            {
                foreach (string fieldTitle in fields)
                {
                    if (!item.listItem.ParentList.Fields.ContainsField(fieldTitle))
                    {
                        continue;
                    }

                    if (item.FieldIsChanged(fieldTitle))
                    {
                        string beforeFieldValue = item.GetFriendlyFieldValue(fieldTitle, false);
                        string afterFieldValue = item.GetFriendlyFieldValue(fieldTitle);

                        if (!BeforeValues && (afterFieldValue == "-" || afterFieldValue == ""))
                        {
                            continue;
                        }

                        if (BeforeValues)
                        {
                            FieldStringTemplate = "<p>{0}: <strike>{1}</strike> {2}</p>";
                        }
                        else 
                        {
                            FieldStringTemplate = "<p>{0}: {2}</p>";
                        }
                        ChangedFieldsBlock += String.Format(FieldStringTemplate, fieldTitle, beforeFieldValue, afterFieldValue);
                    }
                }
            }
            else
            {
                string attachmentUrl = item.listItem.Web.Url + "/" + item.eventProperties.AfterUrl.ToString();
                string attachmentName = Regex.Replace(attachmentUrl, @"^.*\/", "");
                ChangedFieldsBlock += String.Format("<p>{0}: <a href=\"{1}\">{2}</a></p>", "Вложение", attachmentUrl, attachmentName);
            }

            return ChangedFieldsBlock;
        }

        private string CreateBody(ERItem item, List<string> fields)
        {
            string ChangedFieldsBlock;
            string ModifiedByBlock;
            string itemUrlBlock;
            string EditorDisplayName;

            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock(item, fields);

            if (ChangedFieldsBlock == "")
            {
                mailBodyString = "";
            }
            else
            {
                itemUrlBlock = String.Format("<p>Элемент: <a href='{0}'>{1}</a></p>", item.listItem.GetItemFullUrl(), item.itemTitle);

                EditorDisplayName = item.eventProperties.UserDisplayName;
                ModifiedByBlock = String.Format("<p>Кем изменено: {0}</p>", EditorDisplayName);

                mailBodyString = String.Format(ERItem.MAIL_BODY_TEMPLATE, ChangedFieldsBlock, itemUrlBlock, ModifiedByBlock);
            }

            return mailBodyString;
        }

        public void SendMail(SPWeb web)
        {
            if (body != "" && to != "")
            {
                SPUtility.SendEmail(web, headers, body);
            }
        }
    }
}