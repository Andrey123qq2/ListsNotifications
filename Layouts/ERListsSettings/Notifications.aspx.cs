using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ListsNotifications.Layouts.ERListsSettings
{
    public partial class ApplicationPage1 : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageSettings pageSettings = new PageSettings(SettingsPanel, Request, Response);
            pageSettings.CreateSettingsControls();
        }
    }
}
