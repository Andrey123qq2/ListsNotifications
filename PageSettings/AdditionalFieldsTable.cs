using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class AdditionalFieldsTable : SettingsTable
    {
        List<string> tableFields = new List<string> { "cc", "bcc" };
        public AdditionalFieldsTable(PageSettings pageSettings) : base(pageSettings)
        {
        }

        public override TableHeaderRow CreateTableHeader()
        {
            TableHeaderRow header = new TableHeaderRow
            {
                HorizontalAlign = HorizontalAlign.Left
            };

            TableHeaderCell headerTableCell1 = new TableHeaderCell
            {
                Text = "Parameter"
            };
            TableHeaderCell headerTableCell2 = new TableHeaderCell
            {
                Text = "Value",
                Width = 500
            };

            header.Cells.Add(headerTableCell1);
            header.Cells.Add(headerTableCell2);

            return header;
        }

        public override TableRow[] CreateRowsAndFillParams(PageSettings pageSettings)
        {
            List<TableRow> tableRows = new List<TableRow>();

            int i = 0;
            foreach (string field in tableFields)
            {
                i++;
                TableRow tr = new TableRow();

                TableCell td1 = new TableCell();
                TableCell td2 = new TableCell();
                tr.CssClass = (i % 2 == 0) ? "ms-alternatingstrong ms-itmhover" : "ms-itmhover";

                tr.Cells.Add(td1);
                tr.Cells.Add(td2);

                Label fieldLabel1 = new Label
                {
                    ID = "label" + field,
                    Text = field
                };

                List<string> textBoxValueList = (List<string>)pageSettings.ERConf.GetType().GetProperty(field).GetValue(pageSettings.ERConf);
                string textBoxValue = String.Join(",", textBoxValueList.ToArray());
                TextBox textBox1 = new TextBox
                {
                    ID = field,
                    Width = 400,
                    Text = textBoxValue
                };

                td1.Controls.Add(fieldLabel1);
                td2.Controls.Add(textBox1);

                tableRows.Add(tr);
            }

            return tableRows.ToArray();
        }

        public ERConfNotifications GetTableSettings(SPList list)
        {
            List<string> cc = new List<string> { };
            List<string> bcc = new List<string> { };

            ERConfNotifications ERConf = new ERConfNotifications();

            foreach (TableRow tr in table.Rows)
            {
                foreach (TableCell tc in tr.Cells)
                {
                    foreach (Control ctr in tc.Controls)
                    {
                        if (ctr is TextBox)
                        {
                            string ctrID = ((Control)ctr).ID;
                            List<string> ctrIDList = ((TextBox)ctr).Text.Split(',').ToList<string>();
                            ERConf.GetType().GetProperty(ctrID.ToLower()).SetValue(ERConf, ctrIDList);

                            //ERConf[ctrID.ToLower()] = ((TextBox)ctr).Text;
                            //list.RootFolder.Properties["er_notif_" + ctrID.ToLower()] = ((TextBox)ctr).Text;
                            //if (ctrID.Contains("cc")
                            //{
                            //    cc.Add(((TextBox)ctr).Text);
                            //}
                        }
                    }
                }
            };

            //ERConfNotifications ERConf = new ERConfNotifications
            //{
            //    ItemAddedTrackFields = trackFieldsAddedList,
            //    ItemUpdatingTrackFields = trackFieldsList,
            //    ItemUpdatedTrackFields = trackFieldsSingleMail,
            //    to = userNotifyFields
            //};

            return ERConf;
        }
    }
}
