public class GetAllReportRegistryQuery : IRequest<ICollection<ReportRegistryGridModel>>
{
    public class Handler : IRequestHandler<GetAllReportRegistryQuery, ICollection<ReportRegistryGridModel>>
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

        public async Task<ICollection<ReportRegistryGridModel>> Handle(GetAllReportRegistryQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the user's Id from the current HTTP context
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            // Check if the user Id is null or not
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

            var getReportRegistries = await _reportRegistryRepository.GetAllAsync();
            var mapReportRegistries = _mapper.Map<ICollection<ReportRegistryGridModel>>(getReportRegistries);

            return mapReportRegistries;
        }
    }
}