using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class MailNotification
    {
        public MailNotification(NotificationsManager nm)
        {
            nm.ItemEvent += SendMail;
        }
        private StringDictionary GetHeaders(NotificationEventArgs e)
        {
            StringDictionary mailHeaders = new StringDictionary {
                { "to", e.To },
                { "cc", e.Cc },
                { "bcc", e.Bcc },
                { "subject", e.Subject },
                { "content-type", "text/html" }
            };

            return mailHeaders;
        }
        public void SendMail(Object sender, NotificationEventArgs e)
        {
            if (e.Body == "" || (e.To == "" && e.Cc == "" && e.Bcc == ""))
            {
                return;
            }

            StringDictionary headers = GetHeaders(e);

            SPUtility.SendEmail(e.Item.listItem.Web, headers, e.Body);
        }
        public void Unregister(NotificationsManager nm)
        {
            nm.ItemEvent -= SendMail;
        }
    }
}
