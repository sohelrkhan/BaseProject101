namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.FeatureActionMapping.Command
{
    public class CreateFeatureActionMappingCommand : FeatureActionMappingCreateModel, IRequest<FeatureActionMappingCreateModel>
    {
        public class Handler : IRequestHandler<CreateFeatureActionMappingCommand, FeatureActionMappingCreateModel>
        {
            private readonly IFeatureActionMappingRepository _featureActionRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeatureActionMappingRepository featureActionRepository, IHttpContextAccessor httpContextAccessor)
            {
                _featureActionRepository = featureActionRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<FeatureActionMappingCreateModel> Handle(CreateFeatureActionMappingCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                //Check if FeatureActionMapping table has value
                var isExistFeatureAction = await _featureActionRepository.GetFeatureWiseActionsAsync(request.FeatureId);

                var deletedFeatureActions = new List<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.FeatureActionMapping>();

                foreach (var item in isExistFeatureAction)
                {
                    //deletedFeatureActions.Add(item);
                    await _featureActionRepository.DeleteAsync(item);
                }
                // first delete all data
                //await _featureActionRepository.DeleteBulkAsync(isExistFeatureAction);

                foreach (var item in request.ActionIdList)
                {
                    // then insert 
                    var obj = new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.FeatureActionMapping
                    {
                        FeatureId = request.FeatureId,
                        ActionId = item,
                    };

                    var createdfeatureAction = await _featureActionRepository.CreateAsync(obj);
                }

                return request;
            }
        }
    }
}