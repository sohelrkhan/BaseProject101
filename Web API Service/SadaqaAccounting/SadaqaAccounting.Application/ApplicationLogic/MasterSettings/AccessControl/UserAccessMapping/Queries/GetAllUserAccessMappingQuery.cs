namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.UserAccessMapping.Queries
{
    public class GetAllUserAccessMappingQuery : IRequest<ICollection<UserAccessMappingGridModel>>
    {
        public class Handler : IRequestHandler<GetAllUserAccessMappingQuery, ICollection<UserAccessMappingGridModel>>
        {
            private readonly IUserAccessMappingRepository _userAccessRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IUserAccessMappingRepository userAccessManager, IHttpContextAccessor httpContextAccessor)
            {
                _userAccessRepository = userAccessManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<UserAccessMappingGridModel>> Handle(GetAllUserAccessMappingQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var userAccessMappings = await _userAccessRepository.GetAllAsync();

                var groupedData = userAccessMappings
                    .GroupBy(u => new { u.UserId, u.User.FullName })
                    .Select(userGroup => new UserAccessMappingGridModel
                    {
                        UserId = userGroup.Key.UserId,
                        UserName = userGroup.Key.FullName,
                        FeatureActionMappingGridModelList = userGroup
                            .GroupBy(f => new { f.FeatureId, f.Feature.Name })
                            .Select(featureGroup => new FeatureActionMappingGridModel
                            {
                                FeatureId = featureGroup.Key.FeatureId,
                                FeatureName = featureGroup.Key.Name,
                                ActionList = featureGroup.Select(action => new ActionGridModel
                                {
                                    Id = action.Action.Id,
                                    Name = action.Action.Name,
                                    StatusName = action.Action.Status.Name,
                                    IsExist = true,
                                    IsChecked = true
                                }).ToList()
                            }).ToList()
                    }).ToList();

                return groupedData;
            }
        }
    }
}