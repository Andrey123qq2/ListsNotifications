using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Data;
using System.Reflection;
using SPSCommon.SPJsonConf;

namespace ListsNotifications.Layouts.ERListsSettings
{
    public partial class NotificationsSettingsPage : LayoutsPageBase
    {
        private SPList PageSPList;
        private ERConfNotifications ERConf;
        private SPFieldCollection ListFields;
        private PropertyInfo[] ERConfProperties;
        private string CurrentUrl;
        private string MailTemplatesUrl;
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
            ListFields = PageSPList.Fields;

            ERConf = SPJsonConf<ERConfNotifications>.Get(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);

            ERConfProperties = ERConf.GetType().GetProperties();

            //currentUrl = HttpContext.Current.Request.UrlReferrer.OriginalString;
            //currentUrl = HttpContext.Current.Request.Url.OriginalString;
            CurrentUrl = HttpContext.Current.Request.RawUrl;
            MailTemplatesUrl = Regex.Replace(CurrentUrl, "Notifications", "MailTemplates", RegexOptions.IgnoreCase);
        }
        private void BindData()
        {
            AdditionalParamsTable.DataSource = GetDataFoAdditionalTable();
            AdditionalParamsTable.DataBind();

            FieldsTable.DataSource = GetDataForFieldsTable();
            FieldsTable.DataBind();
        }

        private Array GetDataFoAdditionalTable()
        {
            var additionalParamsTableSource = CommonConfigNotif.PAGE_SETTINGS_ADDITIONAL_PARAMS
                .Select(p =>
                {
                    var p1 = new
                    {
                        Parameter = p,
                        Value = String.Join(",", (List<string>)(ERConf.GetType().GetProperty(p).GetValue(ERConf))),
                        LinkVisible = false,
                        LinkValue = ""
                    };
                    return p1;
                })
                .Append(
                    new
                    {
                        Parameter = "Mail Templates",
                        Value = "Configuration",
                        LinkVisible = true,
                        LinkValue = MailTemplatesUrl
                    })
                .ToArray();

            return additionalParamsTableSource;
        }
        private DataTable GetDataForFieldsTable()
        {
            var fieldsDataTable = new DataTable();
            AddColumnsToFieldsDataTable(fieldsDataTable);
            AddDataToFieldsDataTable(fieldsDataTable);

            return fieldsDataTable;
        }
        private void AddColumnsToFieldsDataTable(DataTable fieldsDataTable)
        {
            fieldsDataTable.Columns.Add(new DataColumn("FieldName", typeof(string)));
            fieldsDataTable.Columns.Add(new DataColumn("MailTemplatesUrl", typeof(string)));
            fieldsDataTable.Columns.Add(new DataColumn("UserField", typeof(bool)));

            foreach (var prop in ERConfProperties)
            {
                if (prop.PropertyType != typeof(List<string>))
                    continue;

                fieldsDataTable.Columns.Add(new DataColumn(prop.Name, typeof(bool)));
            }

        }
        private void AddDataToFieldsDataTable(DataTable fieldsDataTable)
        {
            foreach (SPField field in ListFields)
            {
                if (field.ReadOnlyField || field.Hidden)
                    continue;

                List<object> dataRow = new List<object> { };

                string fieldTitle = field.Title;
                
                // Order should be same as in AddColumnsToDataTable
                // data for column FieldName
                dataRow.Add(fieldTitle);
                // data for columt MailTemplatesUrl
                dataRow.Add(MailTemplatesUrl + "&Field=" + fieldTitle);
                // data for column UserField
                dataRow.Add(field.TypeAsString.Contains("User"));

                foreach (var prop in ERConfProperties)
                {
                    if (prop.PropertyType != typeof(List<string>))
                        continue;

                    bool paramContainsField = ((List<string>)(prop.GetValue(ERConf))).Contains(fieldTitle);
                    dataRow.Add(paramContainsField);
                }
                fieldsDataTable.Rows.Add(dataRow.ToArray());
            };
        }

        private void GetAdditionalParamsFromPageToConf()
        {
            var additionalParamsTableRows = AdditionalParamsTable.Rows;

            foreach (GridViewRow row in additionalParamsTableRows)
            {
                string param = ((Label)(row.FindControl("ParameterLabel"))).Text;
                if (!CommonConfigNotif.PAGE_SETTINGS_ADDITIONAL_PARAMS.Contains(param))
                    continue;

                string value = ((TextBox)(row.FindControl("ValueTextBox"))).Text;
                List<string> valueList = Regex.Split(value, @";|,").ToList();

                ERConf.GetType().GetProperty(param).SetValue(ERConf, valueList);
            }
        }
        private void GetFieldsParamsFromPageToConf()
        {
            var fieldsTableRows = FieldsTable.Rows;
            var headerCount = FieldsTable.HeaderRow.Cells.Count;

            for (int i = 1; i < headerCount; i++)
            {
                List<string> valueList = new List<string> { };
                string ctrId = "";
                foreach (GridViewRow row in fieldsTableRows)
                {
                    var cellLabel = row.Cells[0];
                    var fieldName = ((Label)(cellLabel.FindControl("FieldLabel"))).Text;

                    var cell = row.Cells[i];
                    var cellControls = cell.Controls;
                    foreach (var ctr in cellControls)
                    {
                        if (ctr is CheckBox box)
                        {
                            ctrId = box.ID;
                            if (box.Checked)
                            {
                                valueList.Add(fieldName);
                            }
                        }
                    }
                }
                ERConf.GetType().GetProperty(ctrId)?.SetValue(ERConf, valueList);
            }
        }

        protected void TrackSingleMail_EventHandler(object sender, EventArgs e)
        {
            CheckBox senderCheckbox = ((CheckBox)sender);
            senderCheckbox.Parent.FindControl("MailTemplatesUrl").Visible = senderCheckbox.Checked;
        }
        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {

            GetAdditionalParamsFromPageToConf();
            GetFieldsParamsFromPageToConf();

            SPJsonConf<ERConfNotifications>.Set(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF, ERConf);

            RedirectToParentPage();
        }
        protected void ButtonCANCEL_EventHandler(object sender, EventArgs e)
        {
            RedirectToParentPage();
        }

        //TODO: move to common lib
        private void RedirectToParentPage()
        {
            string listSettingsUrl = Regex.Replace(CurrentUrl, "ERListsSettings/Notifications", "listedit", RegexOptions.IgnoreCase);
            Response.Redirect(listSettingsUrl);
        }
        //TODO: move to common lib
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