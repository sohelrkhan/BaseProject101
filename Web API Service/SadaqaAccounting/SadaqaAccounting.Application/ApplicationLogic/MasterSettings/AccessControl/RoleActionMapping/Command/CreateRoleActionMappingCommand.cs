namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.RoleActionMapping.Command
{
    public class CreateRoleActionMappingCommand : RoleActionMappingCreateModel, IRequest<RoleActionMappingCreateModel>
    {
        public class Handler : IRequestHandler<CreateRoleActionMappingCommand, RoleActionMappingCreateModel>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IRoleActionMappingRepository _roleActionMappingRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IRoleRepository roleManager, IRoleActionMappingRepository roleActionMappingManager, IHttpContextAccessor httpContextAccessor)
            {
                _roleRepository = roleManager;
                _roleActionMappingRepository = roleActionMappingManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<RoleActionMappingCreateModel> Handle(CreateRoleActionMappingCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var role = new Role
                {
                    Name = request.RoleName
                };

                var createRole = await _roleRepository.CreateAsync(role);

                foreach (var item in request.FeatureActionMappingCreateModel)
                {
                    foreach (var i in item.ActionIdList)
                    {
                        // then insert 
                        var obj = new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.RoleActionMapping
                        {
                            RoleId = createRole.Id,
                            FeatureId = item.FeatureId,
                            ActionId = i
                        };

                        var createdfeatureAction = await _roleActionMappingRepository.CreateAsync(obj);
                    }
                }

                return request;
            }
        }
    }
}