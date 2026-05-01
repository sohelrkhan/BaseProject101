namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Command
{
    public class ReadAllUnreadNotificationByReceiverIdCommand : IRequest<bool>
    {
        public string ReceiverEmployeeId { get; set; }

        public class Handler : IRequestHandler<ReadAllUnreadNotificationByReceiverIdCommand, bool>
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(INotificationRepository notificationRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(ReadAllUnreadNotificationByReceiverIdCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get all unread notification by login user id
                var unreadNotifications = await _notificationRepository.GetAllUnreadNotificationByReceiverId(request.ReceiverEmployeeId);

                // If have unread notifications then do all read
                if(unreadNotifications is not null && unreadNotifications.Count > 0)
                {
                    foreach (var unreadNotification in unreadNotifications)
                    {
                        unreadNotification.IsRead = true;
                        unreadNotification.UpdatedById = userId;
                        unreadNotification.UpdatedDateTime = DateTime.UtcNow;

                        await _notificationRepository.UpdateAsync(unreadNotification);
                    }

                    return true;
                }

                return false;
            }
        }
    }
}