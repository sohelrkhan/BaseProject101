namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.RoleActionMapping.Queries
{
    public class GetRoleActionMappingByIdQuery : IRequest<RoleActionMappingGridModel>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetRoleActionMappingByIdQuery, RoleActionMappingGridModel>
        {
            private readonly IRoleActionMappingRepository _roleActionMappingManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IRoleActionMappingRepository roleActionMappingRepository, IHttpContextAccessor httpContextAccessor)
            {
                _roleActionMappingManager = roleActionMappingRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<RoleActionMappingGridModel> Handle(GetRoleActionMappingByIdQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getRoleMapping = await _roleActionMappingManager.GetRoleMappingById(request.Id);

                if (getRoleMapping is null)
                    return null!;

                var roleActionMappingGridModel = GetRoleActionMapping.Get(getRoleMapping, request.Id);
                return roleActionMappingGridModel;
            }
        }
    }
}