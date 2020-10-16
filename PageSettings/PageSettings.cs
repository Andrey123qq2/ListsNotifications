using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class PageSettings : IERConf<ERConfNotifications>
    {
        //Table TableSettings;
        Panel SettingsPanel;
        HttpRequest Request;
        HttpResponse Response;

        public SPList list;
        public SPFieldCollection listFields;
        public ERConfNotifications _ERConf;

        TrackFieldsTable trackFieldsTable;
        AdditionalFieldsTable additionalFieldsTable;

        public ERConfNotifications ERConf
        {
            get { return _ERConf; }
        }

        public PageSettings(Panel settingsPanel, HttpRequest request, HttpResponse response)
        {
            SettingsPanel = settingsPanel;
            Request = request;
            Response = response;

            list = GetSPList();
            listFields = list.Fields;
            _ERConf = ERListConf<ERConfNotifications>.Get(list, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);
        }

        public void CreateSettingsControls()
        {
            additionalFieldsTable = new AdditionalFieldsTable(this);
            SettingsPanel.Controls.Add(additionalFieldsTable.table);

            SettingsPanel.Controls.Add(new LiteralControl("<br/>"));

            trackFieldsTable = new TrackFieldsTable(this);
            SettingsPanel.Controls.Add(trackFieldsTable.table);

            Button buttonOK = CreateButtonOK();
            SettingsPanel.Controls.Add(buttonOK);
        }

        private Button CreateButtonOK()
        {
            Button button = new Button
            {
                ID = "OKButton",
                Text = "OK"
            };
            button.Click += new EventHandler(ButtonOK_Click);

            return button;
        }

        private SPList GetSPList()
        {
            SPList list;
            Guid ListGUID = new Guid(Request.QueryString["List"]);

            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    list = web.Lists[ListGUID];
                }
            }

            return list;
        }

        protected void ButtonOK_Click(object sender, System.EventArgs e)
        {
            ERConfNotifications ConfFromAllTables = GetConfFromAllTables();
            ERListConf<ERConfNotifications>.Set(list, CommonConfigNotif.LIST_PROPERTY_JSON_CONF, ConfFromAllTables);

            string currentUrl = HttpContext.Current.Request.UrlReferrer.OriginalString;
            string listSettingsUrl = currentUrl.Replace("ERListsSettings/Notifications.aspx", "listedit.aspx");
            Response.Redirect(listSettingsUrl);
        }
        private ERConfNotifications GetConfFromAllTables()
        {
            ERConfNotifications trackFieldsTableSettings = trackFieldsTable.GetTableSettings(list);
            ERConfNotifications additionalFieldsTableSettings = additionalFieldsTable.GetTableSettings(list);

            trackFieldsTableSettings.cc = additionalFieldsTableSettings.cc;
            trackFieldsTableSettings.bcc = additionalFieldsTableSettings.bcc;

            return trackFieldsTableSettings;
        }
    }
}
