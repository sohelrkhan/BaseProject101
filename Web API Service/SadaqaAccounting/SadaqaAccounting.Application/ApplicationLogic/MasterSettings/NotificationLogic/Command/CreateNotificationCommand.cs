namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Command
{
    public class CreateNotificationCommand : NotificationCreateModel, IRequest<NotificationCreateModel>
    {
        public class Handler : IRequestHandler<CreateNotificationCommand, NotificationCreateModel>
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(INotificationRepository notificationRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<NotificationCreateModel> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdNotification = _mapper.Map<Notifications>(request);
                createdNotification.CreatedById = userId;
                createdNotification.CreatedDateTime = DateTime.UtcNow;

                createdNotification = await _notificationRepository.CreateAsync(createdNotification);
                return request;
            }
        }
    }
}