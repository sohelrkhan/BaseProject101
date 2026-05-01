namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Queries
{
    public class GetSelectListReportGroupQuery : IRequest<IEnumerable<SelectModel>>
    {
        public class Handler : IRequestHandler<GetSelectListReportGroupQuery, IEnumerable<SelectModel>>
        {
            private readonly IReportRegistryRepository _reportRegistryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportRegistryRepository reportRegistryManager, IHttpContextAccessor httpContextAccessor)
            {
                _reportRegistryRepository = reportRegistryManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(GetSelectListReportGroupQuery request, CancellationToken cancellationToken)
            {

                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var reportGroups = await _reportRegistryRepository.GetReportGroupSelectList();
                return reportGroups;
            }
        }
    }
}