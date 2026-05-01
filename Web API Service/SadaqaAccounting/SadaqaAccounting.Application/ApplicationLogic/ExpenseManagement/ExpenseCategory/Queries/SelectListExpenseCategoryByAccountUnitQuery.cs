namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Queries
{
    public class SelectListExpenseCategoryByAccountUnitQuery : IRequest<IEnumerable<SelectModel>>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectListExpenseCategoryByAccountUnitQuery, IEnumerable<SelectModel>>
        {
            private readonly IExpenseCategoryRepository _expenseCategoryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseCategoryRepository expenseCategoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _expenseCategoryRepository = expenseCategoryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListExpenseCategoryByAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExpenseCategories = await _expenseCategoryRepository.GetExpenseCategorySelectListByAccountUnitAsync(request.AccountUnitId, cancellationToken);
                return getExpenseCategories;
            }
        }
    }
}