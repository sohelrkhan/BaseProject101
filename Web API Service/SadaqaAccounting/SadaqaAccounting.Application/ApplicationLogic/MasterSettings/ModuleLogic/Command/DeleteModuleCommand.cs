namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Command
{
    public class DeleteModuleCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteModuleCommand, bool>
        {
            private readonly IModuleRepository _moduleRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IModuleRepository moduleRepository, IHttpContextAccessor httpContextAccessor)
            {
                _moduleRepository = moduleRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDeleteModule = false;
                var existModule = await _moduleRepository.GetByIdAsync(request.Id);

                if (existModule is null)
                    throw new BadRequestException(ProvideErrorMessage.ModuleIdNotFound);

                if (existModule is not null)
                {
                    existModule.IsDeleted = true;
                    existModule.DeletedDateTime = DateTime.UtcNow;

                    var updatedLeavePolicy = await _moduleRepository.UpdateAsync(existModule);
                    isDeleteModule = true;
                }

                return isDeleteModule;
            }
        }
    }
}