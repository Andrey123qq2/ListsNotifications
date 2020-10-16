using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    public interface IERItem
    {
        SPListItem listItem { get; }
        SPItemEventProperties eventProperties { get; }
        string itemTitle { get; }
        string eventType { get; }
    }
}
