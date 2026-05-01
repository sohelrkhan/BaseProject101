namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Queries
{
    public class GetAllIncomeQuery : PaginationRequest, IRequest<PaginatedResponse<IncomeGridModel>>
    {
        public class Handler : IRequestHandler<GetAllIncomeQuery, PaginatedResponse<IncomeGridModel>>
        {
            private readonly IIncomeRepository _incomeRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeRepository incomeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _incomeRepository = incomeRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<IncomeGridModel>> Handle(GetAllIncomeQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getIncomes = await _incomeRepository.GetIncomesFilterAsync(request, cancellationToken);
                var mapIncomes = _mapper.Map<ICollection<IncomeGridModel>>(getIncomes.Data);

                var result = new PaginatedResponse<IncomeGridModel>
                {
                    Data = mapIncomes,
                    CurrentPage = getIncomes.CurrentPage,
                    TotalPages = getIncomes.TotalPages,
                    TotalRecords = getIncomes.TotalRecords,
                    PageSize = getIncomes.PageSize
                };

                return result;
            }
        }
    }
}