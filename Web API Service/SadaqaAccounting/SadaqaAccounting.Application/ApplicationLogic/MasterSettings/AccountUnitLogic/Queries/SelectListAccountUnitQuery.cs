namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Queries
{
    public class SelectListAccountUnitQuery : IRequest<ICollection<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListAccountUnitQuery, ICollection<SelectModel>>
        {
            private readonly IAccountUnitRepository _accountUnitRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IAccountUnitRepository accountUnitRepository, IHttpContextAccessor httpContextAccessor)
            {
                _accountUnitRepository = accountUnitRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<SelectModel>> Handle(SelectListAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getAccountUnits = await _accountUnitRepository.GetAccountUnitSelectListAsync();
                return getAccountUnits;
            }
        }
    }
}
