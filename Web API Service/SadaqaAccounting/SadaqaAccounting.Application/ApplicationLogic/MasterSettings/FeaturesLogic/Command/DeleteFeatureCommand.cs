namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Command
{
    public class DeleteFeatureCommand : IRequest<bool>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteFeatureCommand, bool>
        {
            private readonly IFeaturesRepository _featureRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeaturesRepository featureRepository, IHttpContextAccessor httpContextAccessor)
            {
                _featureRepository = featureRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return false;

                // Decrypt id
                var decryptFeatureId = EncryptionService.Decrypt(request.Id);

                // Check if decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptFeatureId) || string.IsNullOrEmpty(decryptFeatureId))
                    return false;

                // Convert decrypt id
                var convertFeatureId = Convert.ToInt32(decryptFeatureId);

                var isDeleteFeature = false;
                var existFeature = await _featureRepository.GetByIdAsync(convertFeatureId);

                if (existFeature is null)
                    throw new BadRequestException(ProvideErrorMessage.FeatureIdNotFound);

                if (existFeature is not null)
                {
                    existFeature.IsDeleted = true;
                    existFeature.DeletedDateTime = DateTime.UtcNow;

                    var updatedLeavePolicy = await _featureRepository.UpdateAsync(existFeature);
                    isDeleteFeature = true;
                }

                return isDeleteFeature;
            }
        }
    }
}