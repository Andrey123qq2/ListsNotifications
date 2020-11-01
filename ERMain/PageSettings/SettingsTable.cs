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
    abstract class SettingsTable
    {
        public Table table;
        //Dictionary<string, object> FieldsParams;
        public SettingsTable(PageSettings pageSettings)
        {
            table = new Table
            {
                CellPadding = 1,
                CellSpacing = 1,
                BackColor = Color.White,
                BorderColor = Color.Black,
                BorderWidth = 0,
                ForeColor = Color.Black,
                GridLines = GridLines.None,
                BorderStyle = BorderStyle.Dashed
            };

            TableHeaderRow tableHeader = CreateTableHeader();
            table.Rows.Add(tableHeader);

            TableRow[] tableRows = CreateRowsAndFillParams(pageSettings);
            table.Rows.AddRange(tableRows);
        }

        abstract public TableHeaderRow CreateTableHeader();

        abstract public TableRow[] CreateRowsAndFillParams(PageSettings pageSettings);
    }
}
