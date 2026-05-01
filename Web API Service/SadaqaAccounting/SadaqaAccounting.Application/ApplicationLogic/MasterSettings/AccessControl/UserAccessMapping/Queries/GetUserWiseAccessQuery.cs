namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.UserAccessMapping.Queries
{
    public class GetUserWiseAccessQuery : IRequest<UserAccessMappingGridModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetUserWiseAccessQuery, UserAccessMappingGridModel>
        {
            private readonly IUserAccessMappingRepository _userAccessRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IUserAccessMappingRepository userAccessRepository, IHttpContextAccessor httpContextAccessor)
            {
                _userAccessRepository = userAccessRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<UserAccessMappingGridModel> Handle(GetUserWiseAccessQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExistUserAccess = await _userAccessRepository.GetUserWiseAccessAsync(request.Id);

                if (getExistUserAccess is null)
                    return new UserAccessMappingGridModel();

                var userAccessMappingGridModel = GetUserWiseFeatureActionMapping.GetUserWiseFeatureAccessAsync(getExistUserAccess,request.Id);
                return userAccessMappingGridModel;
            }
        }
    }
}