namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Command
{
    public class DeleteReportRegistryCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteReportRegistryCommand, bool>
        {
            private readonly IReportRegistryRepository _reportRegistryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportRegistryRepository reportRegistryManager, IHttpContextAccessor httpContextAccessor)
            {
                _reportRegistryRepository = reportRegistryManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteReportRegistryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isReportRegistryDelete = false;
                var existReportRegistry = await _reportRegistryRepository.GetByIdAsync(request.Id);

                if (existReportRegistry is null)
                    throw new BadRequestException(ProvideErrorMessage.ReportRegistryIdNotFound);

                if (existReportRegistry is not null)
                {
                    existReportRegistry.IsDeleted = true;
                    existReportRegistry.DeletedDateTime = DateTime.UtcNow;

                    var updatedReportRegistry = await _reportRegistryRepository.UpdateAsync(existReportRegistry);
                    isReportRegistryDelete = true;
                }

                return isReportRegistryDelete;
            }
        }
    }
}