namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.FeatureActionMapping.Queries
{
    public class GetAllFeatureWiseActionQuery : IRequest<ICollection<FeatureActionMappingGridModel>>
    {
        public class Handler : IRequestHandler<GetAllFeatureWiseActionQuery, ICollection<FeatureActionMappingGridModel>>
        {
            private readonly IFeatureActionMappingRepository _featureactionRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeatureActionMappingRepository featureactionRepository, IHttpContextAccessor httpContextAccessor)
            {
                _featureactionRepository = featureactionRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<FeatureActionMappingGridModel>> Handle(GetAllFeatureWiseActionQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getAllExistFeatureActon = await _featureactionRepository.GetAllFeatureWiseActionsAsync();

                if (getAllExistFeatureActon.Count == 0)
                    return null!;
                
                var featureActionList = new List<FeatureActionMappingGridModel>();
                foreach (var item in getAllExistFeatureActon.GroupBy(g => g.FeatureId))
                {
                    var actionList = new List<ActionGridModel>();
                    foreach (var action in item)
                    {
                        var model = new ActionGridModel
                        {
                            Id = action.ActionId,
                            Name = action.Action.Name,
                            StatusName = action.Action.Status.Name,
                            IsExist = true,
                            IsChecked = false
                        };
                        actionList.Add(model);
                    }
                    var featureActionGrid = new FeatureActionMappingGridModel
                    {
                        FeatureId = item.Key,
                        FeatureName = item.FirstOrDefault()!.Feature.Name,
                        ActionList = actionList
                    };
                    featureActionList.Add(featureActionGrid);
                }
               
                return featureActionList;
            }
        }
    }
}