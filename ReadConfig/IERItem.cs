using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    interface IERItem
    {
        SPListItem listItem { get; }
        SPItemEventProperties eventProperties { get; }
        string eventType { get; }
        string itemTitle { get; }
    }
}
