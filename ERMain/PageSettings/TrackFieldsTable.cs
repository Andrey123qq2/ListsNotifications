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
                Text = "TrackUpdating",
                Width = 100
            };

            TableHeaderCell headerTableCell3 = new TableHeaderCell
            {
                Text = "TrackAdded",
                Width = 100
            };
            TableHeaderCell headerTableCell4 = new TableHeaderCell
            {
                Text = "Separate Mail",
                Width = 100
            };
            TableHeaderCell headerTableCell5 = new TableHeaderCell
            {
                Text = "Subject For Separate Mail",
                Width = 200
            };
            TableHeaderCell headerTableCell6 = new TableHeaderCell
            {
                Text = "Notify",
                Width = 100
            };
            TableHeaderCell headerTableCell7 = new TableHeaderCell
            {
                Text = "NotifyManagers",
                Width = 100
            };

            header.Cells.Add(headerTableCell1);
            header.Cells.Add(headerTableCell2);
            header.Cells.Add(headerTableCell3);
            header.Cells.Add(headerTableCell4);
            header.Cells.Add(headerTableCell5);
            header.Cells.Add(headerTableCell6);
            header.Cells.Add(headerTableCell7);

            return header;
        }

        public override TableRow[] CreateRowsAndFillParams(PageSettings pageSettings)
        {
            List<TableRow> tableRows = new List<TableRow>();

            int i = 0;
            foreach (SPField field in pageSettings.listFields)
            {
                if (field.ReadOnlyField || field.Hidden)
                {
                    continue;
                };

                i++;

                TableRow tr = new TableRow();

                TableCell td1 = new TableCell();
                TableCell td2 = new TableCell();
                TableCell td3 = new TableCell();
                TableCell td4 = new TableCell();
                TableCell td5 = new TableCell();
                TableCell td6 = new TableCell();
                TableCell td7 = new TableCell();
                tr.CssClass = (i % 2 == 0) ? "ms-alternatingstrong ms-itmhover" : "ms-itmhover";

                tr.Cells.Add(td1);
                tr.Cells.Add(td2);
                tr.Cells.Add(td3);
                tr.Cells.Add(td4);
                tr.Cells.Add(td5);
                tr.Cells.Add(td6);
                tr.Cells.Add(td7);

                Label fieldLabel1 = new Label
                {
                    ID = "labelTrackField" + i.ToString(),
                    Text = field.Title
                };

                CheckBox checkbox1 = new CheckBox
                {
                    ID = "checkBoxTrackField" + i.ToString(),
                    Checked = pageSettings.ERConf.ItemUpdatingTrackFields.Contains(field.Title),
                };
                checkbox1.Attributes.Add("Title", field.Title);

                CheckBox checkbox2 = new CheckBox
                {
                    ID = "checkBoxTrackAddedField" + i.ToString(),
                    Checked = pageSettings.ERConf.ItemAddedTrackFields.Contains(field.Title),
                };
                checkbox2.Attributes.Add("Title", field.Title);

                CheckBox checkbox3 = new CheckBox
                {
                    ID = "checkBoxFieldSingleMail" + i.ToString(),
                    Checked = pageSettings.ERConf.ItemUpdatingTrackFieldsSingleMail.ContainsKey(field.Title),
                };
                checkbox3.Attributes.Add("Title", field.Title);
                checkbox3.Attributes.Add("onclick", "checkBoxSingleMailHandler()");

                TextBox textBox1 = new TextBox
                {
                    ID = "subjectSingleMail" + i.ToString(),
                    Width = 200,
                    Text = (checkbox3.Checked) ? pageSettings.ERConf.ItemUpdatingTrackFieldsSingleMail[field.Title] : "",
                    BorderColor = (checkbox3.Checked) ? Color.FromArgb(171, 171, 171) : Color.FromArgb(225, 225, 225),
                    CssClass = (!checkbox3.Checked) ? "readonly" : ""
                };
                textBox1.Attributes.Add("Title", field.Title);

                CheckBox checkbox4, checkbox5;
                if (field.TypeAsString.Contains("User"))
                {
                    checkbox4 = new CheckBox
                    {
                        ID = "checkBoxFieldNotify" + i.ToString(),
                        Checked = pageSettings.ERConf.to.Contains(field.Title)
                    };
                    checkbox4.Attributes.Add("Title", field.Title);
                    td6.Controls.Add(checkbox4);

                    checkbox5 = new CheckBox
                    {
                        ID = "checkBoxFieldManagersNotify" + i.ToString(),
                        Checked = pageSettings.ERConf.toManagers.Contains(field.Title)
                    };
                    checkbox5.Attributes.Add("Title", field.Title);
                    td7.Controls.Add(checkbox5);
                };

                td1.Controls.Add(fieldLabel1);
                td2.Controls.Add(checkbox1);
                td3.Controls.Add(checkbox2);
                td4.Controls.Add(checkbox3);
                td5.Controls.Add(textBox1);

                tableRows.Add(tr);
            };

            return tableRows.ToArray();
        }

        public ERConfNotifications GetTableSettings(SPList list)
        {
            List<string> trackFieldsList = new List<string> { };
            List<string> trackFieldsAddedList = new List<string> { };
            List<string> userNotifyFields = new List<string> { };
            List<string> userManagersNotifyFields = new List<string> { };
            //List<string> TrackFieldsSingleMail = new List<string> { };
            Dictionary<string, string> trackFieldsSingleMail = new Dictionary<string, string> { };

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
                                trackFieldsSingleMail.Add(controlTitle, textBoxValue);
                                //string singleMailFieldParam = controlTitle + "=" + textBoxValue;
                                ///TrackFieldsSingleMail.Add(singleMailFieldParam);
                            }
                        }

                        if (ctr is CheckBox)
                        {
                            controlTitle = ((CheckBox)ctr).Attributes["Title"].ToString();

                            if (ctrID.Contains("TrackField") && ((CheckBox)ctr).Checked)
                            {
                                trackFieldsList.Add(controlTitle);
                            }

                            if (ctrID.Contains("TrackAddedField") && ((CheckBox)ctr).Checked)
                            {
                                trackFieldsAddedList.Add(controlTitle);
                            }

                            if (ctrID.Contains("FieldNotify") && ((CheckBox)ctr).Checked)
                            {
                                userNotifyFields.Add(controlTitle);
                            }

                            if (ctrID.Contains("FieldManagersNotify") && ((CheckBox)ctr).Checked)
                            {
                                userManagersNotifyFields.Add(controlTitle);
                            }
                        }
                    }
                }
            };

            ERConfNotifications ERConf = new ERConfNotifications
            {
                ItemAddedTrackFields = trackFieldsAddedList,
                ItemUpdatingTrackFields = trackFieldsList,
                ItemUpdatingTrackFieldsSingleMail = trackFieldsSingleMail,
                to = userNotifyFields,
                toManagers = userManagersNotifyFields
            };

            return ERConf;

            //list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS] = String.Join(",", trackFieldsList.ToArray());
            //list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_ITEMADDED] = String.Join(",", trackFieldsAddedList.ToArray());
            //list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_TRACK_FIELDS_SINGLEMAIL] = String.Join(",", TrackFieldsSingleMail.ToArray());
            //list.RootFolder.Properties[NotifCommonConfig.LIST_PROPERTY_USER_FIELDS] = String.Join(",", UserNotifyFields.ToArray());
            //list.Update();
        }
    }
}
