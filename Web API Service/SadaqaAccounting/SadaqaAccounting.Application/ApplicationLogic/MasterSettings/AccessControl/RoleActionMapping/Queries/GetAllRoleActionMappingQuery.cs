namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.RoleActionMapping.Queries
{
    public class GetAllRoleActionMappingQuery : IRequest<ICollection<RoleActionMappingGridModel>>
    {
        public class Handler : IRequestHandler<GetAllRoleActionMappingQuery, ICollection<RoleActionMappingGridModel>>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IRoleActionMappingRepository _roleActionMappingRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IRoleRepository roleRepository, IRoleActionMappingRepository roleActionMappingRepository, IHttpContextAccessor httpContextAccessor)
            {
                _roleRepository = roleRepository;
                _roleActionMappingRepository = roleActionMappingRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<RoleActionMappingGridModel>> Handle(GetAllRoleActionMappingQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var roleActionMappings = await _roleActionMappingRepository.GetAllAsync();

                var groupedData = roleActionMappings
                    .GroupBy(u => new { u.RoleId, u.Role.Name })
                    .Select(roleMapGroup => new RoleActionMappingGridModel
                    {
                        RoleId = roleMapGroup.Key.RoleId,
                        RoleName = roleMapGroup.Key.Name,
                        FeatureActionMappingGridModelList = roleMapGroup
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