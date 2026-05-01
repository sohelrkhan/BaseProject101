namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Command
{
    public class UpdateReportRegistryCommand : ReportRegistryUpdateModel, IRequest<ReportRegistryUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateReportRegistryCommand, ReportRegistryUpdateModel>
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

            public async Task<ReportRegistryUpdateModel> Handle(UpdateReportRegistryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist report registry
                var getExistReportRegistry = await _reportRegistryRepository.GetByIdAsync(request.Id);

                if (getExistReportRegistry is null)
                    throw new BadRequestException(ProvideErrorMessage.ReportRegistryNotFound);

                getExistReportRegistry = _mapper.Map((ReportRegistryUpdateModel)request, getExistReportRegistry);

                getExistReportRegistry = await _reportRegistryRepository.UpdateAsync(getExistReportRegistry);
                return request;
            }
        }
    }
}