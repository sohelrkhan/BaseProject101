namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Queries
{
    public class GetAllUnreadNotificationByReceiverQuery : IRequest<ICollection<NotificationGridModel>>
    {
        public string ReceiverEmployeeId { get; set; }

        public class Handler : IRequestHandler<GetAllUnreadNotificationByReceiverQuery, ICollection<NotificationGridModel>>
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(INotificationRepository notificationRepossitory, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepossitory;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<NotificationGridModel>> Handle(GetAllUnreadNotificationByReceiverQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getNotifications = await _notificationRepository.GetAllUnreadNotificationByReceiverId(request.ReceiverEmployeeId);
                var mapGetNotifications = _mapper.Map<ICollection<NotificationGridModel>>(getNotifications);

                return mapGetNotifications;
            }
        }
    }
}