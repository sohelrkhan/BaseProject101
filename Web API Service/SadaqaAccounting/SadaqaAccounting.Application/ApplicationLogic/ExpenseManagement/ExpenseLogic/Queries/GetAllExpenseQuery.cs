namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Queries
{
    public class GetAllExpenseQuery : PaginationRequest, IRequest<PaginatedResponse<ExpenseGridModel>>
    {
        public class Handler : IRequestHandler<GetAllExpenseQuery, PaginatedResponse<ExpenseGridModel>>
        {
            private readonly IExpenseRepository _expenseRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseRepository expenseRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _expenseRepository = expenseRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<ExpenseGridModel>> Handle(GetAllExpenseQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExpenses = await _expenseRepository.GetExpensesFilterAsync(request, cancellationToken);
                var mapExpenses = _mapper.Map<ICollection<ExpenseGridModel>>(getExpenses.Data);

                var result = new PaginatedResponse<ExpenseGridModel>
                {
                    Data = mapExpenses,
                    CurrentPage = getExpenses.CurrentPage,
                    TotalPages = getExpenses.TotalPages,
                    TotalRecords = getExpenses.TotalRecords,
                    PageSize = getExpenses.PageSize
                };

                return result;
            }
        }
    }
}