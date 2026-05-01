namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Command
{
    public class CreateFeaturesCommand : FeaturesCreateModel, IRequest<FeaturesCreateModel>
    {
        public class Handler : IRequestHandler<CreateFeaturesCommand, FeaturesCreateModel>
        {
            private readonly IFeaturesRepository _featureRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeaturesRepository featureRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _featureRepository = featureRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<FeaturesCreateModel> Handle(CreateFeaturesCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdFeature = _mapper.Map<Feature>(request);
                createdFeature.StatusId = GlobalStatus.Active;
                createdFeature = await _featureRepository.CreateAsync(createdFeature);

                return request;
            }
        }
    }
}