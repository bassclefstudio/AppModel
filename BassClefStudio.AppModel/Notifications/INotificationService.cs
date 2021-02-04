using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Notifications
{
    /// <summary>
    /// Represents a platform service that can show <see cref="NotificationContent"/> notifications to the user.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Shows a notification with the provided <see cref="NotificationContent"/>.
        /// </summary>
        /// <param name="content">The content of the notification.</param>
        /// <returns>A unique ID that can be used to manage the notification.</returns>
        Task<string> ShowNotificationAsync(NotificationContent content);

        /// <summary>
        /// Schedules a notification with the provided <see cref="NotificationContent"/> to be shown at a given time.
        /// </summary>
        /// <param name="content">The content of the notification.</param>
        /// <param name="showTime">The <see cref="DateTimeOffset"/> when the notification will be sent.</param>
        /// <returns>A unique ID that can be used to manage the scheduled alarm notifications.</returns>
        Task<string> ShowAlarmAsync(NotificationContent content, DateTimeOffset showTime);

        /// <summary>
        /// Attempts to cancel the scheduled notification with the given ID.
        /// </summary>
        /// <param name="id">The <see cref="string"/> ID of the notification to cancel - see <see cref="ShowNotificationAsync(NotificationContent)"/> and <see cref="ShowAlarmAsync(NotificationContent, DateTimeOffset)"/> for how to get this ID.</param>
        /// <returns>A <see cref="bool"/> indicating that the operation succeeded.</returns>
        Task<bool> CancelAlarmAsync(string id);
    }

    /// <summary>
    /// Represents the content of a notification shown be a <see cref="INotificationService"/>.
    /// </summary>
    public class NotificationContent
    {
        /// <summary>
        /// The title of the notification.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The lines of body text of the notification.
        /// </summary>
        public string[] Body { get; }

        /// <summary>
        /// Creates a new <see cref="NotificationContent"/>.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="body">The lines of body text of the notification.</param>
        public NotificationContent(string title, params string[] body)
        {
            Title = title;
            Body = body;
        }
    }
}
