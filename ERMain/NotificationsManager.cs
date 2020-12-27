using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class NotificationsManager
    {
		public event EventHandler<NotificationEventArgs> ItemEvent;

		public static void SendNotifications(ERItemNotifications itemER)
		{
			NotificationEventArgs notificationEventArgs = itemER.NotificationEventArgs;

			NotificationsManager notificationsManager = new NotificationsManager();
			notificationsManager.OnNewEvent(notificationEventArgs);
		}
		public NotificationsManager()
		{
			NotificationsManager nm = new NotificationsManager();

			MailNotification mailNotification = new MailNotification(nm);
			mailNotification.Unregister(nm);
		}
		public void OnNewEvent(NotificationEventArgs e)
		{
			EventHandler<NotificationEventArgs> temp = Volatile.Read(ref ItemEvent);

			temp?.Invoke(this, e);
		}
	}
}