namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Queries
{
    public class GetNotificationDetailQuery : IRequest<NotificationUpdateModel>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetNotificationDetailQuery, NotificationUpdateModel>
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

            public async Task<NotificationUpdateModel> Handle(GetNotificationDetailQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist notification by id
                var getExistNotification = await _notificationRepository.GetByIdAsync(request.Id);

                if (getExistNotification is null)
                    return new NotificationUpdateModel();

                // Map exist company id
                var mapExitNotification = _mapper.Map<NotificationUpdateModel>(getExistNotification);

                return mapExitNotification;
            }
        }
    }
}