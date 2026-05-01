namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Command
{
    public class DeleteActionCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteActionCommand, bool>
        {
            private readonly IActionRepository _actionRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IActionRepository actionRepository, IHttpContextAccessor httpContextAccessor)
            {
                _actionRepository = actionRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteActionCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDeleteAction = false;
                var existAction = await _actionRepository.GetByIdAsync(request.Id);

                if (existAction is null)
                    throw new BadRequestException(ProvideErrorMessage.ActionIdNotFound);

                if (existAction is not null)
                {
                    existAction.IsDeleted = true;
                    existAction.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _actionRepository.UpdateAsync(existAction);
                    isDeleteAction = true;
                }

                return isDeleteAction;
            }
        }
    }
}