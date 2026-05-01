namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Queries
{
    public class GetAllBankQuery : PaginationRequest, IRequest<PaginatedResponse<BankGridModel>>
    {
        public class Handler : IRequestHandler<GetAllBankQuery, PaginatedResponse<BankGridModel>>
        {
            private readonly IBankRepository _bankRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IBankRepository bankRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _bankRepository = bankRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<BankGridModel>> Handle(GetAllBankQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getBanks = await _bankRepository.GetBanksFilterAsync(request, cancellationToken);
                var mapBanks = _mapper.Map<ICollection<BankGridModel>>(getBanks.Data);

                var result = new PaginatedResponse<BankGridModel>
                {
                    Data = mapBanks,
                    CurrentPage = getBanks.CurrentPage,
                    TotalPages = getBanks.TotalPages,
                    TotalRecords = getBanks.TotalRecords,
                    PageSize = getBanks.PageSize
                };

                return result;
            }
        }
    }
}