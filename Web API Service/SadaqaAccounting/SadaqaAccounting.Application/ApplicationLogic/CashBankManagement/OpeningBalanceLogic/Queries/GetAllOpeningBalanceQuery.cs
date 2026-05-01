namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.OpeningBalanceLogic.Queries
{
    public class GetAllOpeningBalanceQuery : PaginationRequest, IRequest<PaginatedResponse<OpeningBalanceGridModel>>
    {
        public class Handler : IRequestHandler<GetAllOpeningBalanceQuery, PaginatedResponse<OpeningBalanceGridModel>>
        {
            private readonly IOpeningBalanceRepository _openingBalanceRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IOpeningBalanceRepository openingBalanceRepository, 
                IMapper mapper, 
                IHttpContextAccessor httpContextAccessor)
            {
                _openingBalanceRepository = openingBalanceRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<OpeningBalanceGridModel>> Handle(GetAllOpeningBalanceQuery request, 
                CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getOpeningBalance = await _openingBalanceRepository.GetOpeningBalancesFilterAsync(request, cancellationToken);
                var mapOpeningBalance = _mapper.Map<ICollection<OpeningBalanceGridModel>>(getOpeningBalance.Data);

                var result = new PaginatedResponse<OpeningBalanceGridModel>
                {
                    Data = mapOpeningBalance,
                    CurrentPage = getOpeningBalance.CurrentPage,
                    TotalPages = getOpeningBalance.TotalPages,
                    TotalRecords = getOpeningBalance.TotalRecords,
                    PageSize = getOpeningBalance.PageSize
                };

                return result;
            }
        }
    }
}