using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SPSCommon.ERItem;
using SPSCommon.SPCustomExtensions;

namespace ListsNotifications
{
    internal sealed class NotificationEventArgs : EventArgs
    {
        public readonly string Body;
        public readonly string Subject;
        public string To;
        public string Cc;
        public string Bcc;
        public IERItem Item;

        private readonly string AttachmentUrl;
        private readonly bool ShowBeforeValues;
        private readonly string ModifiedByBlockTemplate;
        private string ItemUrlBlock;
        private string EditorDisplayName;
        private string ModifiedByBlock;
        private readonly Dictionary<string, string> TemplatesParams;

        public NotificationEventArgs(
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
        }

        public NotificationEventArgs(
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
        }

        private void InitCommonAttributes(ERItemNotifications item)
        {
            To = String.Join(",", item.ToMails);
            Cc = String.Join(",", item.ERConf.cc);
            Bcc = String.Join(",", item.ERConf.bcc);
            Item = item;

            ItemUrlBlock = String.Format(TemplatesParams["MAIL_URL_TEMPLATE"], item.listItem.GetItemFullUrl(), item.itemTitle);
            EditorDisplayName = item.eventProperties.UserDisplayName;
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
            string fieldStringTemplate = ShowBeforeValues ? TemplatesParams["MAIL_FIELDS_TEMPLATE_ITEMS_BEFORE"] : 
                TemplatesParams["MAIL_FIELDS_TEMPLATE_ITEMS_NOTBEFORE"];

            foreach (SPItemField field in itemFields)
            {
                // TODO: Move condition block to another/field class
                if (!ShowBeforeValues && (field.FriendlyFieldValueAfter == "-" || field.FriendlyFieldValueAfter == ""))
                {
                    continue;
                }

                ChangedFieldsBlock += String.Format(
                    fieldStringTemplate, 
                    field.FieldTitle, 
                    field.FriendlyFieldValueBefore, 
                    field.FriendlyFieldValueAfter
                );
            }

            return ChangedFieldsBlock;
        }
        private string GetChangedFieldsBlock()
        {
            string attachmentName = Regex.Replace(AttachmentUrl, @"^.*\/", "");
            string ChangedFieldsBlock = String.Format(
                TemplatesParams["MAIL_FIELDS_TEMPLATE_ATTACHMENTS"], 
                TemplatesParams["MAIL_BODY_ATTACHMENTS"], 
                AttachmentUrl, 
                attachmentName
            );

            return ChangedFieldsBlock;
        }
        private string CreateBody(List<SPItemField> fields)
        {
            string ChangedFieldsBlock;
            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock(fields);

            mailBodyString = ChangedFieldsBlock != "" ? String.Format(
                TemplatesParams["MAIL_BODY_TEMPLATE"], 
                ChangedFieldsBlock, 
                ItemUrlBlock, 
                ModifiedByBlock
            ) : "";

            return mailBodyString;
        }

        private string CreateBody()
        {
            string ChangedFieldsBlock;
            string mailBodyString;

            ChangedFieldsBlock = GetChangedFieldsBlock();
            mailBodyString = String.Format(
                TemplatesParams["MAIL_BODY_TEMPLATE"], 
                ChangedFieldsBlock, 
                ItemUrlBlock, 
                ModifiedByBlock
            );

            return mailBodyString;
        }

    }
}
