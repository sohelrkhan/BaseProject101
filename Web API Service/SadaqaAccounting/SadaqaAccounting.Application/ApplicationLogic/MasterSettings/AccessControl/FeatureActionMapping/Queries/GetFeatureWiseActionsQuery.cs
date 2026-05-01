namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.FeatureActionMapping.Queries
{
    public class GetFeatureWiseActionsQuery : IRequest<FeatureActionMappingGridModel>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetFeatureWiseActionsQuery, FeatureActionMappingGridModel>
        {
            private readonly IFeatureActionMappingRepository _featureactionRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IFeatureActionMappingRepository featureactionRepository, IHttpContextAccessor httpContextAccessor)
            {
                _featureactionRepository = featureactionRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<FeatureActionMappingGridModel> Handle(GetFeatureWiseActionsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExistFeatureActon = await _featureactionRepository.GetFeatureWiseActionsAsync(request.Id);

                if (getExistFeatureActon is null)
                    return null!;
                var actionList = new List<ActionGridModel>();
                foreach (var item in getExistFeatureActon)
                {
                    var model = new ActionGridModel
                    {
                        Id = item.ActionId,
                        Name = item.Action.Name,
                        StatusName = item.Action.Status.Name,
                        IsExist = true
                    };
                    actionList.Add(model);
                }

                var featureActionGrid = new FeatureActionMappingGridModel
                {
                    FeatureId = request.Id,
                    ActionList = actionList
                };
                return featureActionGrid;
            }
        }
    }
}