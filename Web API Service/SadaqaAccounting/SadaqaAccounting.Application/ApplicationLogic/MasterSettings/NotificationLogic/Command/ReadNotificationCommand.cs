namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Command
{
    public class ReadNotificationCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<ReadNotificationCommand, bool>
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(INotificationRepository notificationRepositoiry, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepositoiry;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist notification
                var getNotification = await _notificationRepository.GetByIdAsync(request.Id);

                if (getNotification is null)
                    throw new BadRequestException(ProvideErrorMessage.NotificationNotFound);

                getNotification.IsRead = true;
                getNotification.UpdatedById = userId;
                getNotification.UpdatedDateTime = DateTime.UtcNow;

                getNotification = await _notificationRepository.UpdateAsync(getNotification);
                return true;
            }
        }
    }
}