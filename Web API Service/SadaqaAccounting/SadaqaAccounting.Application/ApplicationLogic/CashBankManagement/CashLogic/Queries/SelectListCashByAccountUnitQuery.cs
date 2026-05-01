namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.CashLogic.Queries
{
    public class SelectListCashByAccountUnitQuery : IRequest<IEnumerable<SelectModel>>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectListCashByAccountUnitQuery, IEnumerable<SelectModel>>
        {
            private readonly ICashRepository _cashRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(ICashRepository cashRepository, IHttpContextAccessor httpContextAccessor)
            {
                _cashRepository = cashRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListCashByAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getCashes = await _cashRepository.GetCashSelectListByAccountUnitAsync(request.AccountUnitId, cancellationToken);
                return getCashes;
            }
        }
    }
}