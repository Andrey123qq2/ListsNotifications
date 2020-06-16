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
    class MailNotification
    {
        public readonly string body;
        public readonly string to;
        //public readonly string cc;
        //public readonly string bcc;
        public readonly string subject;
        private SPListItem listItem;
        StringDictionary headers;

        public MailNotification(SPListItem item, List<string> fields, List<SPPrincipal> principals)
        {
            listItem = item;
            body = CreateBody(item, fields);
            to = String.Join(",", SPCommon.GetUserMails(principals) );
            subject = String.Format("{0}: элемент изменен", item.Title);
            headers = GetHeaders();
        }

        private StringDictionary GetHeaders()
        {
            StringDictionary mailHeaders = new StringDictionary(); ;
            mailHeaders.Add("to", to);
            //mailHeaders.Add("cc", cc);
            //mailHeaders.Add("bcc", bcc);
            mailHeaders.Add("subject", subject);

            mailHeaders.Add("content-type", "text/html");
            return mailHeaders;
        }

        private string GetChangedFieldsBlock(SPListItem item, List<string> fields)
        {
            string ChangedFieldsBlock = "";
            foreach (string fieldTitle in fields)
            {
                if (!item.ParentList.Fields.ContainsField(fieldTitle))
                {
                    continue;
                }

                if (item.FieldIsChanged(fieldTitle))
                {
                    string beforeFieldValue = item.GetFriendlyFieldValue(fieldTitle, false);
                    string afterFieldValue = item.GetFriendlyFieldValue(fieldTitle);
                    ChangedFieldsBlock += String.Format("<p>{0}: <strike>{1}</strike> {2}</p>", fieldTitle, beforeFieldValue, afterFieldValue);
                }
            }

            return ChangedFieldsBlock;
        }

        private string CreateBody(SPListItem item, List<string> fields)
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
                itemUrlBlock = String.Format("<p>Элемент: <a href='{0}'>{1}</a></p>", item.GetItemFullUrl(), item.Title);

                EditorDisplayName = item.GetEventProperties().UserDisplayName;
                ModifiedByBlock = String.Format("<p>Кем изменено: {0}</p>", EditorDisplayName);

                mailBodyString = String.Format(ConfigParams.MAIL_BODY_TEMPLATE, ChangedFieldsBlock, itemUrlBlock, ModifiedByBlock);
            }

            return mailBodyString;
        }

        public void SendMail()
        {
            if (body != "" && to != "")
            SPUtility.SendEmail(listItem.ParentList.ParentWeb, headers, body);
        }
    }
}