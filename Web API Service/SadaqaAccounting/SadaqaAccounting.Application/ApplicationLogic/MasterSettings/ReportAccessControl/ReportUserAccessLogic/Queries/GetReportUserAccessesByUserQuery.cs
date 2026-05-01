namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportUserAccessLogic.Queries
{
    public class GetReportUserAccessesByUserQuery : IRequest<ICollection<ReportRegistryUserAccessGridModel>>
    {
        public string UserId { get; set; }
        public class Handler : IRequestHandler<GetReportUserAccessesByUserQuery, ICollection<ReportRegistryUserAccessGridModel>>
        {
            private readonly IReportUserAccessRepository _reportUserAccessRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportUserAccessRepository reportUserAccessRepository, IHttpContextAccessor httpContextAccessor)
            {
                _reportUserAccessRepository = reportUserAccessRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<ReportRegistryUserAccessGridModel>> Handle(GetReportUserAccessesByUserQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getReportUserAccesses = await _reportUserAccessRepository.GetReportUserAccessesByUserAsync(request.UserId);
                var reportRegistryUserAccessGridModels = new List<ReportRegistryUserAccessGridModel>();

                foreach (var reportUserAccess in getReportUserAccesses)
                {
                    var reportRegistryUserAccessGridModel = new ReportRegistryUserAccessGridModel
                    {
                        ReportId = reportUserAccess.ReportRegistryId,
                        ReportName = reportUserAccess.ReportRegistry.Name,
                        ModuleName = reportUserAccess.ReportRegistry.Module.Name,
                        ReportCode = reportUserAccess.ReportRegistry.ReportCode,
                        ReportGroup = reportUserAccess.ReportRegistry.ReportGroup,
                        UserId = request.UserId,
                        IsChecked = true
                    };
                    reportRegistryUserAccessGridModels.Add(reportRegistryUserAccessGridModel);
                }

                return reportRegistryUserAccessGridModels;
            }
        }
    }
}