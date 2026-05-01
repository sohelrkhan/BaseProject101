namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Command
{
    public class DeleteExpenseCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteExpenseCategoryCommand, bool>
        {
            private readonly IExpenseCategoryRepository _expenseCategoryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseCategoryRepository expenseCategoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _expenseCategoryRepository = expenseCategoryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteExpenseCategoryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDeleteAction = false;
                var existExpenseCategory = await _expenseCategoryRepository.GetByIdAsync(request.Id);

                if (existExpenseCategory is null)
                    throw new BadRequestException(ProvideErrorMessage.ExpenseCategoryIdNotFound);

                if (existExpenseCategory is not null)
                {
                    existExpenseCategory.IsDeleted = true;
                    existExpenseCategory.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _expenseCategoryRepository.UpdateAsync(existExpenseCategory);
                    isDeleteAction = true;
                }

                return isDeleteAction;
            }
        }
    }
}