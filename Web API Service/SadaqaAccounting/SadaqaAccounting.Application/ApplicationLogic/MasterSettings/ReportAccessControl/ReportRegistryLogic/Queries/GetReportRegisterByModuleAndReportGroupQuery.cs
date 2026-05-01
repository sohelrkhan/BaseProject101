namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Queries
{
    public class GetReportRegisterByModuleAndReportGroupQuery : IRequest<IEnumerable<ReportRegistryGridModel>>
    {
        public int? ModuleId { get; set; }
        public string? ReportGroupName { get; set; }

        public class Handler : IRequestHandler<GetReportRegisterByModuleAndReportGroupQuery, IEnumerable<ReportRegistryGridModel>>
        {
            private readonly IReportRegistryRepository _reportRegistryRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportRegistryRepository reportRegistryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _reportRegistryRepository = reportRegistryRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<ReportRegistryGridModel>> Handle(GetReportRegisterByModuleAndReportGroupQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get report registries based on moduleId and ReportGroupName
                var reportRegistries = await _reportRegistryRepository.GetReportRegistersByModuleIdAndReportGroupName(request.ModuleId, request.ReportGroupName);
                var mapReportRegistries = _mapper.Map<IEnumerable<ReportRegistryGridModel>>(reportRegistries);

                return mapReportRegistries;
            }
        }
    }
}