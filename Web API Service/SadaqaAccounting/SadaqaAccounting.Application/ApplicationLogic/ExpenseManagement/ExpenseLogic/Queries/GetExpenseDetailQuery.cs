namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Queries
{
    public class GetExpenseDetailQuery : IRequest<ExpenseUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetExpenseDetailQuery, ExpenseUpdateModel>
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

            public async Task<ExpenseUpdateModel> Handle(GetExpenseDetailQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if expense id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new ExpenseUpdateModel();

                // Decrypt expense id
                var decryptExpenseId = EncryptionService.Decrypt(request.Id);

                // Check if expense decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptExpenseId) || string.IsNullOrEmpty(decryptExpenseId))
                    return new ExpenseUpdateModel();

                // Convert decrypt expense id
                var convertExpenseId = Convert.ToInt32(decryptExpenseId);

                var getExistExpenseCategory = await _expenseRepository.GetByIdAsync(convertExpenseId);

                if (getExistExpenseCategory is null)
                    return new ExpenseUpdateModel();

                var mapExitExpense = _mapper.Map<ExpenseUpdateModel>(getExistExpenseCategory);
                mapExitExpense.ExpenseDateString = Convert.ToDateTime(getExistExpenseCategory.ExpenseDate).ToString("dd-MM-yyyy");

                return mapExitExpense;
            }
        }
    }
}