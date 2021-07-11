using FluentValidation.Results;
using SocialPay.Helper.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SocialPay.Helper.Models.NotificationModel;

namespace SocialPay.Helper.Notification
{
    public interface INotification
    {
        IReadOnlyCollection<NotificationModel> Notifications { get; }
        bool HasNotifications { get; }
        void AddNotification(string key, string message, ENotificationType notificationType);
        void AddNotification(string key, string message);
        void AddNotifications(IReadOnlyCollection<NotificationModel> notifications);
        void AddNotifications(IList<NotificationModel> notifications);
        void AddNotifications(ICollection<NotificationModel> notifications);
        void AddNotifications(ValidationResult validationResult);
    }
}
