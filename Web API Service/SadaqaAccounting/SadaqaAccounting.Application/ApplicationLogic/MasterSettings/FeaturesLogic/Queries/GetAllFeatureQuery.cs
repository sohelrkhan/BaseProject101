namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Queries
{
    public class GetAllFeatureQuery : IRequest<IQueryable<FeaturesGridModel>>
    {
        public class Handler : IRequestHandler<GetAllFeatureQuery, IQueryable<FeaturesGridModel>>
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

            public async Task<IQueryable<FeaturesGridModel>> Handle(GetAllFeatureQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getFeatures = _featureRepository.GetAllAsync();
                var mapGetFeatures = _mapper.Map<List<FeaturesGridModel>>(getFeatures).AsQueryable();

                return mapGetFeatures;
            }
        }
    }
}