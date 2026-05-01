namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Queries
{
    public class SelectListFeatureQuery : IRequest<ICollection<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListFeatureQuery, ICollection<SelectModel>>
        {
            private readonly IFeaturesRepository _featureRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeaturesRepository featureRepository, IHttpContextAccessor httpContextAccessor)
            {
                _featureRepository = featureRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<SelectModel>> Handle(SelectListFeatureQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getFeatures = await _featureRepository.GetFeatureSelectListAsync(cancellationToken);
                return getFeatures;
            }
        }
    }
}