namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Command
{
    public class UpdateExpenseCategoryCommand : ExpenseCategoryUpdateModel, IRequest<ExpenseCategoryUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateExpenseCategoryCommand, ExpenseCategoryUpdateModel>
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

            public async Task<ExpenseCategoryUpdateModel> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Get exist expense category
                var getExistExpenseCategory = await _expenseCategoryRepository.GetByIdAsync(request.Id);

                if (getExistExpenseCategory is null)
                    throw new BadRequestException(ProvideErrorMessage.ExpenseCategoryIdNotFound);

                getExistExpenseCategory = _mapper.Map((ExpenseCategoryUpdateModel)request, getExistExpenseCategory);
                getExistExpenseCategory.AccountUnitId = accountUnitId;

                getExistExpenseCategory = await _expenseCategoryRepository.UpdateAsync(getExistExpenseCategory);

                return request;
            }
        }
    }
}