namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserAccountUnitLogic.Query
{
    public class GetUserAccountUnitQuery : IRequest<UserAccountUnitGridModel>
    {
        public string UserId { get; set; }

        public class Handler: IRequestHandler<GetUserAccountUnitQuery,UserAccountUnitGridModel>
        {
            private readonly IUserAccountUnitRepository _userAccountUnitRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IUserAccountUnitRepository userAccountUnitRepository, 
                IMapper mapper, 
                IHttpContextAccessor httpContextAccessor)
            {
                _userAccountUnitRepository = userAccountUnitRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<UserAccountUnitGridModel> Handle(
                GetUserAccountUnitQuery request, 
                CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getUserAccountUnits = await _userAccountUnitRepository.GetUserAccountUnitSelectListAsync(request.UserId);

                if (getUserAccountUnits is null)
                    return new UserAccountUnitGridModel();

                var accountUnitList = new List<AccountUnitGridModel>();
                foreach (var item in getUserAccountUnits)
                {
                    var model = new AccountUnitGridModel
                    {
                        Id = item.AccountUnitId,
                        Name = item.AccountUnit.Name,
                        IsExist = true
                    };
                    accountUnitList.Add(model);
                }

                var userAccountUnitGrid = new UserAccountUnitGridModel
                {
                    UserId = request.UserId,
                    AccountUnitList = accountUnitList
                };
                
                return userAccountUnitGrid;
            }
        }
    }
}