using SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Model;

namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Queries
{
    public class GetAllAccountUnitQuery: IRequest<ICollection<AccountUnitGridModel>>
    {
        public class Handler : IRequestHandler<GetAllAccountUnitQuery, ICollection<AccountUnitGridModel>>
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

            public async Task<ICollection<AccountUnitGridModel>> Handle(GetAllAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getAccountUnits = await _accountUnitRepository.GetAllAsync();
                var mapGetAccountUnits = _mapper.Map<ICollection<AccountUnitGridModel>>(getAccountUnits);

                return mapGetAccountUnits;
            }
        }
    }
}
