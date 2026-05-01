namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Queries
{
    public class GetAllExpenseCategoryQuery : PaginationRequest, IRequest<PaginatedResponse<ExpenseCategoryGridModel>>
    {
        public class Handler : IRequestHandler<GetAllExpenseCategoryQuery, PaginatedResponse<ExpenseCategoryGridModel>>
        {
            private readonly IExpenseCategoryRepository _expenseCategoryRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseCategoryRepository _expenseCategoryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this._expenseCategoryRepository = _expenseCategoryRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<ExpenseCategoryGridModel>> Handle(GetAllExpenseCategoryQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExpenseCategories = await _expenseCategoryRepository.GetExpenseCategoriesFilterAsync(request, cancellationToken);
                var mapExpenseCategories = _mapper.Map<ICollection<ExpenseCategoryGridModel>>(getExpenseCategories.Data);

                var result = new PaginatedResponse<ExpenseCategoryGridModel>
                {
                    Data = mapExpenseCategories,
                    CurrentPage = getExpenseCategories.CurrentPage,
                    TotalPages = getExpenseCategories.TotalPages,
                    TotalRecords = getExpenseCategories.TotalRecords,
                    PageSize = getExpenseCategories.PageSize
                };

                return result;
            }
        }
    }
}