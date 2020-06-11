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
            to = GetToMails(principals);
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
        private string GetToMails(List<SPPrincipal> principalsList)
        {
            List<string> toMailsList = new List<string>();
            string toMails;

            foreach (SPPrincipal principal in principalsList)
            {
                if (principal.GetType().Name != "SPUser")
                {
                    continue;
                }
                SPUser user = (SPUser)principal;
                toMailsList.Add(user.Email);
            }
            toMails = String.Join(",", toMailsList);
            return toMails;
        }

        private string CreateBody(SPListItem item, List<string> fields)
        {
            string ChangedFieldsBlock = "";
            string ModifiedByBlock;
            //List<string> EditorField = new List<string> { "Editor" };
            string EditorDisplayName;
            string itemUrlBlock;
            string mailBodyString;

            foreach (string fieldTitle in fields)
            {
                if (!item.ParentList.Fields.ContainsField(fieldTitle))
                {
                    continue;
                }

                if (item.NotUserFieldIsChanged(fieldTitle))
                {
                    string changeFieldValue = item.GetChangedFieldValue(fieldTitle);
                    if (SPCommon.IsUTCDateString(changeFieldValue))
                    {
                        changeFieldValue = DateTime.Parse(changeFieldValue).ToLocalTime().ToString();
                    }
                    ChangedFieldsBlock += String.Format("<p>{0}: {1}</p>", fieldTitle, changeFieldValue);
                }

            }
            if (ChangedFieldsBlock == "")
            {
                return "";
            }

            itemUrlBlock = String.Format("<p>Элемент: <a href='{0}'>{1}</a></p>", item.GetItemFullUrl(), item.Title);

            //EditorDisplayName = item.GetUsersFromUsersFields(EditorField)[0].Name;
            EditorDisplayName = item.GetEventProperties().UserDisplayName;
            ModifiedByBlock = String.Format("<p>Кем изменено: {0}</p>", EditorDisplayName );

            mailBodyString = String.Format(ConfigParams.MAIL_BODY_TEMPLATE, ChangedFieldsBlock, itemUrlBlock, ModifiedByBlock);

            return mailBodyString;
        }

        public void SendMail()
        {
            if (body != "" && to != "")
            SPUtility.SendEmail(listItem.ParentList.ParentWeb, headers, body);
        }
    }
}