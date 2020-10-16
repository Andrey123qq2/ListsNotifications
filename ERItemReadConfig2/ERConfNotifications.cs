using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class ERConfNotifications
    {
        public List<string> to { get; set; } = new List<string> { };
        public List<string> cc { get; set; } = new List<string> { };
        public List<string> bcc { get; set; } = new List<string> { };
        public List<string> ItemAddedTrackFields { get; set; } = new List<string> { };
        public List<string> ItemUpdatingTrackFields { get; set; } = new List<string> { };
        public Dictionary<string, string> ItemUpdatingTrackFieldsSingleMail { get; set; } = new Dictionary<string, string> { };
    }
}
