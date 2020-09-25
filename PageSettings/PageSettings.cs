using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ListsNotifications
{
    class PageSettings
    {
        //Table TableSettings;
        Panel SettingsPanel;
        HttpRequest Request;
        HttpResponse Response;

        public SPList list;
        public SPFieldCollection listFields;
        public ERItem listERConfig;

        TrackFieldsTable trackFieldsTable;
        AdditionalFieldsTable additionalFieldsTable;

        public PageSettings(Panel settingsPanel, HttpRequest request, HttpResponse response)
        {
            SettingsPanel = settingsPanel;
            Request = request;
            Response = response;

            list = GetSPList();
            listFields = list.Fields;
            listERConfig = ERItemFactory.Create(list);
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
            trackFieldsTable.SaveTableSettings(list);
            additionalFieldsTable.SaveTableSettings(list);

            string currentUrl = HttpContext.Current.Request.UrlReferrer.OriginalString;
            string listSettingsUrl = currentUrl.Replace("ERListsSettings/ERListsSettings.aspx", "listedit.aspx");
            Response.Redirect(listSettingsUrl);
        }
    }
}
