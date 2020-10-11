using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    public class ERConfNotifications0
    {
        public List<string> cc { get; set; } = new List<string> { };
        public List<string> bcc { get; set; } = new List<string> { };
        public List<string> to { get; set; } = new List<string> { };
        public List<string> ItemAddedTrackFields { get; set; } = new List<string> { };
        public List<string> ItemUpdatedTrackFields { get; set; } = new List<string> { };
        public Dictionary<string, string> ItemUpdatingTrackFields { get; set; } = new Dictionary<string, string> { };
    }
}
