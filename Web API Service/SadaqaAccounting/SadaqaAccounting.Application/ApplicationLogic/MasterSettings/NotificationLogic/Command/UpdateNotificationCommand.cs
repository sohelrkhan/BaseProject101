namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Command
{
    public class UpdateNotificationCommand : NotificationUpdateModel, IRequest<NotificationUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateNotificationCommand, NotificationUpdateModel>
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

            public async Task<NotificationUpdateModel> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
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

                getNotification = _mapper.Map((UpdateNotificationCommand)request, getNotification);
                getNotification.UpdatedById = userId;
                getNotification.UpdatedDateTime = DateTime.UtcNow;

                getNotification = await _notificationRepository.UpdateAsync(getNotification);
                return request;
            }
        }
    }
}