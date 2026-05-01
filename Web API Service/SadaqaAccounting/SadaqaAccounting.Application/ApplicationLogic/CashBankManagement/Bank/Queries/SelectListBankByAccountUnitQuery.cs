namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Queries
{
    public class SelectListBankByAccountUnitQuery : IRequest<IEnumerable<SelectModel>>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectListBankByAccountUnitQuery, IEnumerable<SelectModel>>
        {
            private readonly IBankRepository _bankRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IBankRepository bankRepository, IHttpContextAccessor httpContextAccessor)
            {
                _bankRepository = bankRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListBankByAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getBanks = await _bankRepository.GetBankSelectListByAccountUnitAsync(request.AccountUnitId, cancellationToken);
                return getBanks;
            }
        }
    }
}