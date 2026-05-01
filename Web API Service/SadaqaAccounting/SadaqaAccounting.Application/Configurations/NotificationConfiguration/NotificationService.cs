namespace SadaqaAccounting.Application.Configurations.NotificationConfiguration
{
    public static class NotificationService
    {
        public static async Task<bool> SendNotificationAsync(INotificationRepository _notificationRepository,string receiverUserId, string senderUserId, string url,string featureName)
        {
            var createNotification = new Notifications
            {
                Title = $"{featureName} Notification",
                Message = $"You have a notification for Approve {featureName}.",
                ReceiverUserId = receiverUserId,
                SenderUserId = senderUserId,
                Url = url,
                IsRead = false,
                CreatedById = senderUserId,
                CreatedDateTime = DateTime.UtcNow,
            };

            var isCreate = await _notificationRepository.CreateAsync(createNotification);

            if (isCreate != null)
                return true;

            return false;
        }
    }
}