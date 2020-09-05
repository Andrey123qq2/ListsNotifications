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
    class TrackFieldsTable : SettingsTable
    {
        public TrackFieldsTable(PageSettings pageSettings) : base(pageSettings)
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
                Text = "FieldName"
            };
            TableHeaderCell headerTableCell2 = new TableHeaderCell
            {
                Text = "Track",
                Width = 100
            };
            TableHeaderCell headerTableCell3 = new TableHeaderCell
            {
                Text = "Separate Mail",
                Width = 100
            };
            TableHeaderCell headerTableCell4 = new TableHeaderCell
            {
                Text = "Subject For Separate Mail",
                Width = 200
            };
            TableHeaderCell headerTableCell5 = new TableHeaderCell
            {
                Text = "Notify",
                Width = 100
            };

            header.Cells.Add(headerTableCell1);
            header.Cells.Add(headerTableCell2);
            header.Cells.Add(headerTableCell3);
            header.Cells.Add(headerTableCell4);
            header.Cells.Add(headerTableCell5);

            return header;
        }

        public override TableRow[] CreateRowsAndFillParams(SPFieldCollection listFields, ERItem listERConfig)
        {
            List<TableRow> tableRows = new List<TableRow>();

            int i = 0;
            foreach (SPField field in listFields)
            {
                i++;

                if (field.ReadOnlyField || field.Hidden)
                {
                    continue;
                };

                TableRow tr = new TableRow();

                TableCell td1 = new TableCell();
                TableCell td2 = new TableCell();
                TableCell td3 = new TableCell();
                TableCell td4 = new TableCell();
                TableCell td5 = new TableCell();
                tr.CssClass = (i % 2 == 0) ? "ms-alternatingstrong ms-itmhover" : "ms-itmhover";

                tr.Cells.Add(td1);
                tr.Cells.Add(td2);
                tr.Cells.Add(td3);
                tr.Cells.Add(td4);
                tr.Cells.Add(td5);

                Label fieldLabel1 = new Label
                {
                    ID = "labelTrackField" + i.ToString(),
                    Text = field.Title
                };

                CheckBox checkbox1 = new CheckBox
                {
                    ID = "checkBoxTrackField" + i.ToString(),
                    Checked = listERConfig.TrackFields.Contains(field.Title),
                };
                checkbox1.Attributes.Add("Title", field.Title);

                CheckBox checkbox2 = new CheckBox
                {
                    ID = "checkBoxFieldSingleMail" + i.ToString(),
                    Checked = listERConfig.TrackFieldsSingleMail.ContainsKey(field.Title),
                };
                checkbox2.Attributes.Add("Title", field.Title);
                checkbox2.Attributes.Add("onclick", "checkBoxSingleMailHandler()");

                TextBox textBox1 = new TextBox
                {
                    ID = "subjectSingleMail" + i.ToString(),
                    Width = 200,
                    Text = (checkbox2.Checked) ? listERConfig.TrackFieldsSingleMail[field.Title] : "",
                    BorderColor = (checkbox2.Checked) ? Color.FromArgb(171, 171, 171) : Color.FromArgb(225, 225, 225),
                    CssClass = (!checkbox2.Checked) ? "readonly" : ""
                };
                textBox1.Attributes.Add("Title", field.Title);

                CheckBox checkbox3;
                if (field.TypeAsString.Contains("User"))
                {
                    checkbox3 = new CheckBox
                    {
                        ID = "checkBoxFieldNotify" + i.ToString(),
                        Checked = listERConfig.UserNotifyFields.Contains(field.Title)
                    };
                    checkbox3.Attributes.Add("Title", field.Title);
                    td5.Controls.Add(checkbox3);
                };

                td1.Controls.Add(fieldLabel1);
                td2.Controls.Add(checkbox1);
                td3.Controls.Add(checkbox2);
                td4.Controls.Add(textBox1);

                tableRows.Add(tr);
            };

            return tableRows.ToArray();
        }

        public void SaveTableSettings(SPList list)
        {
            List<string> trackFieldsList = new List<string> { };
            List<string> UserNotifyFields = new List<string> { };
            List<string> TrackFieldsSingleMail = new List<string> { };

            foreach (TableRow tr in table.Rows)
            {
                foreach (TableCell tc in tr.Cells)
                {
                    foreach (Control ctr in tc.Controls)
                    {
                        string controlTitle;
                        string ctrID = ((Control)ctr).ID;

                        if (ctr is TextBox)
                        {
                            controlTitle = ((TextBox)ctr).Attributes["Title"].ToString();
                            string textBoxValue = ((TextBox)ctr).Text;

                            if (ctrID.Contains("SingleMail") && textBoxValue != "")
                            {
                                string singleMailFieldParam = controlTitle + "=" + textBoxValue;
                                TrackFieldsSingleMail.Add(singleMailFieldParam);
                            }
                        }

                        if (ctr is CheckBox)
                        {
                            controlTitle = ((CheckBox)ctr).Attributes["Title"].ToString();

                            if (ctrID.Contains("TrackField") && ((CheckBox)ctr).Checked)
                            {
                                trackFieldsList.Add(controlTitle);
                            }

                            if (ctrID.Contains("FieldNotify") && ((CheckBox)ctr).Checked)
                            {
                                UserNotifyFields.Add(controlTitle);
                            }
                        }
                    }
                }
            };

            list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS] = String.Join(",", trackFieldsList.ToArray());
            list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL] = String.Join(",", TrackFieldsSingleMail.ToArray());
            list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_USER_FIELDS] = String.Join(",", UserNotifyFields.ToArray());
            list.Update();
        }
    }
}
