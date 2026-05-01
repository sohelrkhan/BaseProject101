namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.FeatureActionMapping.Queries
{
    public class GetFeatureActionDetailQuery : IRequest<FeatureActionMappingUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetFeatureActionDetailQuery, FeatureActionMappingUpdateModel>
        {
            private readonly IFeatureActionMappingRepository _featureActionMappingRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeatureActionMappingRepository featureActionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _featureActionMappingRepository = featureActionRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<FeatureActionMappingUpdateModel> Handle(GetFeatureActionDetailQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if feature id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new FeatureActionMappingUpdateModel();

                // Decrypt action id
                var decryptActionId = EncryptionService.Decrypt(request.Id);

                // Check if action decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptActionId) || string.IsNullOrEmpty(decryptActionId))
                    return new FeatureActionMappingUpdateModel();

                // Convert decrypt action id
                var convertFeatureActionMappingId = Convert.ToInt32(decryptActionId);

                var getExistFeatureAction = await _featureActionMappingRepository.GetByIdAsync(convertFeatureActionMappingId);

                if (getExistFeatureAction is null)
                    return new FeatureActionMappingUpdateModel();

                var mapExitFeatureAction = _mapper.Map<FeatureActionMappingUpdateModel>(getExistFeatureAction);
                return mapExitFeatureAction;
            }
        }
    }
}