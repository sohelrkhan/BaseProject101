namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Command
{
    public class DeleteExpenseCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteExpenseCommand, bool>
        {
            private readonly IExpenseRepository _expenseRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseRepository expenseRepository, IHttpContextAccessor httpContextAccessor)
            {
                _expenseRepository = expenseRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDeleteAction = false;
                var existExpense = await _expenseRepository.GetByIdAsync(request.Id);

                if (existExpense is null)
                    throw new BadRequestException(ProvideErrorMessage.ExpenseIdNotFound);

                if (existExpense is not null)
                {
                    existExpense.IsDeleted = true;
                    existExpense.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _expenseRepository.UpdateAsync(existExpense);
                    isDeleteAction = true;
                }

                return isDeleteAction;
            }
        }
    }
}