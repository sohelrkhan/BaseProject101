namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Command
{
    public class CreateExpenseCategoryCommand : ExpenseCategoryCreateModel, IRequest<ExpenseCategoryCreateModel>
    {
        public class Handler : IRequestHandler<CreateExpenseCategoryCommand, ExpenseCategoryCreateModel>
        {
            private readonly IExpenseCategoryRepository _expenseCategoryRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseCategoryRepository expenseCategoryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _expenseCategoryRepository = expenseCategoryRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ExpenseCategoryCreateModel> Handle(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdExpenseCategory = _mapper.Map<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory>(request);
                createdExpenseCategory.AccountUnitId = accountUnitId;
                createdExpenseCategory.CreatedById = userId;
                createdExpenseCategory.CreatedDateTime = DateTime.UtcNow;

                createdExpenseCategory = await _expenseCategoryRepository.CreateAsync(createdExpenseCategory);
                return request;
            }
        }
    }
}