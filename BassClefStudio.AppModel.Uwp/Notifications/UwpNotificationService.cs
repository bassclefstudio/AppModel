using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace BassClefStudio.AppModel.Notifications
{
    /// <summary>
    /// The UWP/Action Center implementation of <see cref="INotificationService"/> which can show Windows 10 notifications.
    /// </summary>
    public class UwpNotificationService : INotificationService
    {
        internal ToastNotifier ToastNotifier { get; }
        public UwpNotificationService()
        {
            ToastNotifier = ToastNotificationManager.CreateToastNotifier();
        }

        //public ScheduledToastNotification[] GetScheduledNotifications()
        //{
        //    return ToastNotifier.GetScheduledToastNotifications().ToArray();
        //}

        /// <inheritdoc/>
        public async Task<string> ShowAlarmAsync(NotificationContent content, DateTimeOffset showTime)
        {
            var notification = new ScheduledToastNotification(content.GetXml(), showTime);
            string tag = Guid.NewGuid().ToString();
            notification.Tag = tag;
            notification.Group = "Alarms";
            ToastNotifier.AddToSchedule(notification);
            return tag;
        }

        /// <inheritdoc/>
        public async Task<string> ShowNotificationAsync(NotificationContent content)
        {
            var notification = new ToastNotification(content.GetXml());
            string tag = Guid.NewGuid().ToString();
            notification.Tag = tag;
            notification.Group = "Toasts";
            ToastNotifier.Show(notification);
            return tag;
        }

        /// <inheritdoc/>
        public async Task<bool> CancelAlarmAsync(string id)
        {
            var toast = ToastNotifier.GetScheduledToastNotifications().FirstOrDefault(t => t.Id == id);
            if(toast != null)
            {
                ToastNotifier.RemoveFromSchedule(toast);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// UWP extension methods for the <see cref="NotificationContent"/> class.
    /// </summary>
    public static class NotificationContentExtensions
    {
        public static XmlDocument GetXml(this NotificationContent content)
        {
            var builder = new ToastContentBuilder();
            builder.AddText(content.Title, AdaptiveTextStyle.Title);
            foreach (var l in content.Body)
            {
                builder.AddText(l, AdaptiveTextStyle.Body);
            }
            return builder.Content.GetXml();
        }
    }
}
