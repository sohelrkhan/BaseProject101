public class GetReportRegistryDetailsQuery : IRequest<ReportRegistryUpdateModel>
{
    public int Id { get; set; }

    public class Handler : IRequestHandler<GetReportRegistryDetailsQuery, ReportRegistryUpdateModel>
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

        public async Task<ReportRegistryUpdateModel> Handle(GetReportRegistryDetailsQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the user's Id from the current HTTP context
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            // Check if the user Id is null or not
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

            // Check if report registry id is null
            if (request.Id == -1)
                return new ReportRegistryUpdateModel();

            var getExistReportRegistry = await _reportRegistryRepository.GetByIdAsync(request.Id);

            if (getExistReportRegistry is null)
                return new ReportRegistryUpdateModel();

            var mapReportRegistry = _mapper.Map<ReportRegistryUpdateModel>(getExistReportRegistry);
            return mapReportRegistry;
        }
    }
}