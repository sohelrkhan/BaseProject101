namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Command
{
    public class UpdateFeaturesCommand : FeaturesUpdateModel, IRequest<FeaturesUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateFeaturesCommand, FeaturesUpdateModel>
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

            public async Task<FeaturesUpdateModel> Handle(UpdateFeaturesCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist features
                var getExistFeature = await _featureRepository.GetByIdAsync(request.Id);

                if (getExistFeature is null)
                    throw new BadRequestException(ProvideErrorMessage.FeatureNotFound);

                getExistFeature = _mapper.Map((FeaturesUpdateModel)request, getExistFeature);
                getExistFeature = await _featureRepository.UpdateAsync(getExistFeature);

                return request;
            }
        }
    }
}