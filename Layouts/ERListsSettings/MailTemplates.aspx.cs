using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPERCommonLib;
using System.Linq;
using System.Collections;

namespace ListsNotifications.Layouts.ERListsSettings
{
    public partial class MailTemplatesPage : LayoutsPageBase
    {
        private string FieldName;
        private SPList PageSPList;
        private ERConfNotifications ERConf;
        private string ERConfKey;
        private string FieldNameLabelText;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitParams();

            if (IsPostBack)
            {
                return;
            }

            FieldNameLabel.Text = String.Format(FieldNameLabel.Text, FieldNameLabelText);

            BindData();
        }

        private void BindData()
        {
            Dictionary<string, string> templatesValues;

            if (!ERConf.MailTemplates.ContainsKey(ERConfKey))
            {
                templatesValues = CommonConfigNotif.MAIL_TEMPLATES_DEFAULT;
            }
            else 
            {
                templatesValues = ERConf.MailTemplates[ERConfKey];
            }

            MailVariablesTable.DataSource = GetTemplatesValues(templatesValues);

            MailVariablesTable.DataBind();
        }

        private object GetTemplatesValues(Dictionary<string, string> templates)
        {
            var defaultTemplatesValues = templates
                .Select(k =>
                {
                    var textMode = k.Value.Length > 100 ? TextBoxMode.MultiLine : TextBoxMode.SingleLine;

                    return new
                    {
                        Variable = k.Key,
                        Value = k.Value,
                        TextMode = textMode,
                    };
                })
                .ToArray();

            return defaultTemplatesValues;
        }
        private void InitParams()
        {
            FieldName = Request.QueryString["Field"];
            Guid listGuid = new Guid(Request.QueryString["List"]);

            PageSPList = GetSPList(listGuid);

            ERConf = ERListConf<ERConfNotifications>.Get(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);

            ERConfKey = (!String.IsNullOrEmpty(FieldName)) ? FieldName : "_listMode";

            FieldNameLabelText = (!String.IsNullOrEmpty(FieldName)) ? "field \"" + FieldName + "\"" : "list";
        }

        private Dictionary<string, string> GetMailTemplatesConfFromPage()
        {
            IEnumerator ie = MailVariablesTable.Rows.GetEnumerator();
            Dictionary<string, string> mailParams = new Dictionary<string, string> { };

            while (ie.MoveNext())
            {
                var row = (GridViewRow)ie.Current;
                string variable = ((Label)(row.FindControl("TextBoxLabel"))).Text;
                string value = ((TextBox)(row.FindControl("TextBox"))).Text;

                mailParams.Add(variable, value);

            }
            ie.Reset();

            return mailParams;
        }

        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {
            ERConf.MailTemplates[ERConfKey] = GetMailTemplatesConfFromPage();

            ERListConf<ERConfNotifications>.Set(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF, ERConf);

            RedirectToParentPage();
        }

        protected void ButtonCANCEL_EventHandler(object sender, EventArgs e)
        {
            RedirectToParentPage();
        }

        private void RedirectToParentPage()
        {
            string currentUrl = HttpContext.Current.Request.UrlReferrer.OriginalString;
            string listSettingsUrl = Regex.Replace(currentUrl, "MailTemplates", "Notifications", RegexOptions.IgnoreCase);
            Response.Redirect(listSettingsUrl);
        }

        private SPList GetSPList(Guid listGUID)
        {
            SPList list;

            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    list = web.Lists[listGUID];
                }
            }

            return list;
        }
    }
}
