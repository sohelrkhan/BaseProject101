namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Queries
{
    public class GetAccountUnitDetailQuery : IRequest<AccountUnitUpdateModel>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetAccountUnitDetailQuery, AccountUnitUpdateModel>
        {
            private readonly IAccountUnitRepository _accountUnitRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IAccountUnitRepository accountUnitRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _accountUnitRepository = accountUnitRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<AccountUnitUpdateModel> Handle(GetAccountUnitDetailQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getAccountUnit = await _accountUnitRepository.GetByIdAsync(request.Id);

                if (getAccountUnit is null)
                    return new AccountUnitUpdateModel();

                var mapAccountUnit = _mapper.Map<AccountUnitUpdateModel>(getAccountUnit);
                return mapAccountUnit;
            }
        }
    }
}