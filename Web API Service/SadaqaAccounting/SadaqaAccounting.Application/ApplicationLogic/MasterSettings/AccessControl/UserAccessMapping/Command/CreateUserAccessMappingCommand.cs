namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.UserAccessMapping.Command
{
    public class CreateUserAccessMappingCommand : UserAccessMappingCreateModel, IRequest<UserAccessMappingCreateModel>
    {
        public class Handler : IRequestHandler<CreateUserAccessMappingCommand, UserAccessMappingCreateModel>
        {
            private readonly IUserAccessMappingRepository _userAccessRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IUserAccessMappingRepository userAccessRepository, IHttpContextAccessor httpContextAccessor)
            {
                _userAccessRepository = userAccessRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<UserAccessMappingCreateModel> Handle(CreateUserAccessMappingCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                //Check if UserAccessMapping table has value
                var isExistAccess = await _userAccessRepository.GetUserWiseAccessAsync(request.UserId);

                foreach (var item in isExistAccess)
                    await _userAccessRepository.DeleteAsync(item);

                foreach (var item in request.FeatureActionMappingCreateModel)
                {
                    foreach (var i in item.ActionIdList)
                    {
                        // then insert 
                        var obj = new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.UserAccessMapping
                        {
                            UserId = request.UserId,
                            FeatureId = item.FeatureId,
                            ActionId = i
                        };

                        var createdfeatureAction = await _userAccessRepository.CreateAsync(obj);
                    }
                }

                return request;
            }
        }
    }
}