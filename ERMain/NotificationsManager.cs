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
			List<NotificationEventArgs> notificationEventArgs = itemER.EventArgs;

			NotificationsManager notificationsManager = new NotificationsManager();
			MailNotification mailNotification = new MailNotification(notificationsManager);

			notificationsManager.OnNewEvent(notificationEventArgs);

			mailNotification.Unregister(notificationsManager);
		}
		public void OnNewEvent(List<NotificationEventArgs> eventArgs)
		{
			EventHandler<NotificationEventArgs> temp = Volatile.Read(ref ItemEvent);

			eventArgs.ForEach(e => temp?.Invoke(this, e));
		}
	}
}