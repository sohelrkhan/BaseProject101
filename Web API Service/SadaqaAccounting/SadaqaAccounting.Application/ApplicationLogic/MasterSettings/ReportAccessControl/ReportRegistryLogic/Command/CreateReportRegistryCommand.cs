namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Command
{
    public class CreateReportRegistryCommand : ReportRegistryCreateModel, IRequest<ReportRegistryCreateModel>
    {
        public class Handler : IRequestHandler<CreateReportRegistryCommand, ReportRegistryCreateModel>
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

            public async Task<ReportRegistryCreateModel> Handle(CreateReportRegistryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var existReport = await _reportRegistryRepository.GetByReportCodeAsync(request.ReportCode);

                if (existReport is not null)
                    throw new Exception(request.ReportCode + " - Report code already exist! Try new one.");

                var createdReportRegistry = _mapper.Map<ReportRegistry>(request);
                createdReportRegistry.StatusId = GlobalStatus.Active;

                createdReportRegistry = await _reportRegistryRepository.CreateAsync(createdReportRegistry);
                return request;
            }
        }
    }
}