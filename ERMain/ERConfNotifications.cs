using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    /// <summary>
    /// Configuration of Event Receiver stores in JSON format and it reads/writes from/to this object
    /// </summary>
    class ERConfNotifications
    {
        public List<string> to { get; set; } = new List<string> { };
        public List<string> toManagers { get; set; } = new List<string> { };
        public List<string> cc { get; set; } = new List<string> { };
        public List<string> bcc { get; set; } = new List<string> { };
        public List<string> ItemAddedTrackFields { get; set; } = new List<string> { };
        public List<string> ItemUpdatingTrackFields { get; set; } = new List<string> { };
        public List<string> TrackFieldsSingleMail { get; set; } = new List<string> { };

        // ItemUpdatingTrackFieldsSingleMail is deprecated, should be removed in future
        public Dictionary<string, string> ItemUpdatingTrackFieldsSingleMail { get; set; } = new Dictionary<string, string> { };
        public List<string> ItemUpdatingFixedFields { get; set; } = new List<string> { };
        public Dictionary<string, Dictionary<string, string>> MailTemplates { get; set; } = new Dictionary<string, Dictionary<string, string>> { };
    }
}
