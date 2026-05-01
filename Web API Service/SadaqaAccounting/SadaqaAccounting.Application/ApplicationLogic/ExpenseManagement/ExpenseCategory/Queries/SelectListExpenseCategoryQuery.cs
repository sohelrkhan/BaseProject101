namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Queries
{
    public class SelectListExpenseCategoryQuery : IRequest<List<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListExpenseCategoryQuery, List<SelectModel>>
        {
            private readonly IExpenseCategoryRepository _expenseCategoryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseCategoryRepository expenseCategoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _expenseCategoryRepository = expenseCategoryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<List<SelectModel>> Handle(SelectListExpenseCategoryQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExpenseCategories = await _expenseCategoryRepository.GetExpenseCategorySelectListAsync();

                var expenseCategorySelectList = getExpenseCategories
                    .Select(s => new SelectModel()
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();

                return expenseCategorySelectList;
            }
        }
    }
}