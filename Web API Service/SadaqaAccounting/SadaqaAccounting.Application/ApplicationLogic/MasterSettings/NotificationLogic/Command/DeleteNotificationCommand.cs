namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Command
{
    public class DeleteNotificationCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteNotificationCommand, bool>
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(INotificationRepository notificationRepository, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isCompanyDelete = false;
                var existNotification = await _notificationRepository.GetByIdAsync(request.Id);

                if (existNotification is null)
                    throw new BadRequestException(ProvideErrorMessage.NotificationIdNotFound);

                if (existNotification is not null)
                {
                    existNotification.IsDeleted = true;
                    existNotification.DeletedDateTime = DateTime.UtcNow;

                    var updatedCompany = await _notificationRepository.UpdateAsync(existNotification);
                    isCompanyDelete = true;
                }

                return isCompanyDelete;
            }
        }
    }
}