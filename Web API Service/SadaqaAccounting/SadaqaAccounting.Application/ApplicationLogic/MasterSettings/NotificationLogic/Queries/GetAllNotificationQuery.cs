namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Queries
{
    public class GetAllNotificationQuery : IRequest<ICollection<NotificationGridModel>>
    {
        public class Handler : IRequestHandler<GetAllNotificationQuery, ICollection<NotificationGridModel>>
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

            public async Task<ICollection<NotificationGridModel>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getNotifications = await _notificationRepository.GetAllAsync();
                var mapGetNotifications = _mapper.Map<ICollection<NotificationGridModel>>(getNotifications);

                return mapGetNotifications;
            }
        }
    }
}