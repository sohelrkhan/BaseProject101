namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserAccountUnitLogic.Command
{
    public class CreateUserAccountUnitCommand: UserAccountUnitCreateModel, IRequest<UserAccountUnitCreateModel>
    {
        public class Handler : IRequestHandler<CreateUserAccountUnitCommand, UserAccountUnitCreateModel>
        {
            private readonly IUserAccountUnitRepository _userAccountUnitRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IUserAccountUnitRepository userAccountUnitRepository, IHttpContextAccessor httpContextAccessor)
            {
                _userAccountUnitRepository = userAccountUnitRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<UserAccountUnitCreateModel> Handle(CreateUserAccountUnitCommand request, CancellationToken cancellationToken)
            {
                var deleteUserAccountUnits = new List<UserAccountUnit>();
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                //Check if User Account Unit table has value
                var isExistAccountUnit = await _userAccountUnitRepository.GetUserAccountUnitSelectListAsync(request.UserId);

                foreach (var item in isExistAccountUnit)
                {
                    item.IsDeleted = true;
                    item.DeletedDateTime = DateTime.UtcNow;
                    deleteUserAccountUnits.Add(item);
                }
                // first delete all data
                await _userAccountUnitRepository.UpdateBulkAsync(deleteUserAccountUnits);

                foreach (var item in request.AccountUnitIds)
                {
                    // then insert 
                    var obj = new UserAccountUnit
                    {
                        AccountUnitId = item,
                        UserId = request.UserId
                    };

                    var createdUserAccountUnit = await _userAccountUnitRepository.CreateAsync(obj);
                }
                return request;
            }
        }
    }
}
