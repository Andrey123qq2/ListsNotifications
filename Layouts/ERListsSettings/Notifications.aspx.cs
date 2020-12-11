using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPERCommonLib;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web.UI.WebControls;

namespace ListsNotifications.Layouts.ERListsSettings
{
    //TODO: recreate aspx page with static elements (tables etc) and dynamically set its contents
    public partial class NotificationsSettingsPage : LayoutsPageBase
    {
        private SPList PageSPList;
        private ERConfNotifications ERConf;
        private SPFieldCollection listFields;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitParams();

            if (IsPostBack)
            {
                return;
            }

            BindData();
        }

        private void InitParams()
        {
            Guid listGuid = new Guid(Request.QueryString["List"]);

            PageSPList = GetSPList(listGuid);
            listFields = PageSPList.Fields;

            ERConf = ERListConf<ERConfNotifications>.Get(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);
        }

        private void BindData()
        {

            AdditionalParamsTable.DataSource = GetAdditionalParamsFromConfToTableSource();
            AdditionalParamsTable.DataBind();

            FieldsTable.DataSource = GetFieldsTableSource();
            FieldsTable.DataBind();
        }

        private Array GetAdditionalParamsFromConfToTableSource()
        {
            var additionalParamsTableSource = CommonConfigNotif.PAGE_SETTINGS_ADDITIONAL_PARAMS
                .Select(p => {
                    return new
                    {
                        Parameter = p,
                        Value = String.Join(",", (List<string>)(ERConf.GetType().GetProperty(p).GetValue(ERConf)))
                    };
                })
                .ToArray();

            return additionalParamsTableSource;
        }

        private Array GetFieldsTableSource()
        {
            List<object> fieldsTableSource = new List<object> { };
            int i = 0;
            foreach (SPField field in listFields)
            {
                if (field.ReadOnlyField || field.Hidden)
                {
                    continue;
                };
                i++;
                string fieldTitle = field.Title;

                fieldsTableSource.Add(
                    new
                    {
                        FieldName = fieldTitle,
                        TrackUpdating = ERConf.ItemUpdatingTrackFields.Contains(fieldTitle),
                        TrackAdded = ERConf.ItemAddedTrackFields.Contains(fieldTitle),
                        SeparateMail = ERConf.ItemUpdatingTrackFieldsSingleMail.Contains(fieldTitle),
                        Notify = ERConf.to.Contains(fieldTitle),
                        NotifyManagers = ERConf.toManagers.Contains(fieldTitle),
                        FixedUpdating = ERConf.ItemUpdatingFixedFields.Contains(fieldTitle)
                    }
                );
            };

            return fieldsTableSource.ToArray();
        }

        private void GetAdditionalParamsConfFromPage()
        {
            IEnumerator ie = AdditionalParamsTable.Rows.GetEnumerator();
            //List<string> additionalParams = new List<string> { };

            while (ie.MoveNext())
            {
                var row = (GridViewRow)ie.Current;
                string param = ((Label)(row.FindControl("ParameterLabel"))).Text;
                string value = ((TextBox)(row.FindControl("ValueTextBox"))).Text;
                List<string> valueList = Regex.Split(value, @";|,").ToList();

                ERConf.GetType().GetProperty(param).SetValue(ERConf, valueList);
                //additionalParams.Add(variable, value);

            }

            //ie.Reset();

            //return additionalParams;
        }
        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {
            //ERConf.MailTemplates[ERConfKey] = GetMailTemplatesConfFromPage();

            GetAdditionalParamsConfFromPage();

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
            string listSettingsUrl = Regex.Replace(currentUrl, "ERListsSettings/Notifications", "listedit", RegexOptions.IgnoreCase);
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
