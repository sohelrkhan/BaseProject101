namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportUserAccessLogic.Queries
{
    public class GetAllReportRegistryUserAccessQuery : IRequest<ICollection<ReportRegistryUserAccessGridModel>>
    {
        public class Handler : IRequestHandler<GetAllReportRegistryUserAccessQuery, ICollection<ReportRegistryUserAccessGridModel>>
        {
            private readonly IReportRegistryRepository _reportRegistryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportRegistryRepository reportRegistryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _reportRegistryRepository = reportRegistryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<ReportRegistryUserAccessGridModel>> Handle(GetAllReportRegistryUserAccessQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getReportRegistries = await _reportRegistryRepository.GetAllAsync();
                var reportRegistryUserAccessGridModels = new List<ReportRegistryUserAccessGridModel>();

                foreach (var getReportRegistry in getReportRegistries)
                {
                    var reportRegistryUserAccessGridModel = new ReportRegistryUserAccessGridModel
                    {
                        ReportId = getReportRegistry.Id,
                        ReportName = getReportRegistry.Name,
                        ModuleName = getReportRegistry.Module.Name,
                        ReportCode = getReportRegistry.ReportCode,
                        ReportGroup = getReportRegistry.ReportGroup,
                        UserId = "",
                        IsChecked = false
                    };
                    reportRegistryUserAccessGridModels.Add(reportRegistryUserAccessGridModel);
                }

                return reportRegistryUserAccessGridModels;
            }
        }
    }
}