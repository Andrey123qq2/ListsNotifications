using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    public class ERItemListItemNullException : Exception
    {
        public ERItemListItemNullException()
        {
        }

        public ERItemListItemNullException(string message)
            : base(message)
        {
        }

        public ERItemListItemNullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
