namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Queries
{
    public class GetExpenseCategoryDetailsQuery : IRequest<ExpenseCategoryUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetExpenseCategoryDetailsQuery, ExpenseCategoryUpdateModel>
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

            public async Task<ExpenseCategoryUpdateModel> Handle(GetExpenseCategoryDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if feature id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new ExpenseCategoryUpdateModel();

                // Decrypt expense category id
                var decryptExpenseCategoryId = EncryptionService.Decrypt(request.Id);

                // Check if expense category decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptExpenseCategoryId) || string.IsNullOrEmpty(decryptExpenseCategoryId))
                    return new ExpenseCategoryUpdateModel();

                // Convert decrypt expense category id
                var convertExpenseCategoryId = Convert.ToInt32(decryptExpenseCategoryId);

                var getExistExpenseCategory = await _expenseCategoryRepository.GetByIdAsync(convertExpenseCategoryId);

                if (getExistExpenseCategory is null)
                    return new ExpenseCategoryUpdateModel();

                var mapExitExpenseCategory = _mapper.Map<ExpenseCategoryUpdateModel>(getExistExpenseCategory);
                return mapExitExpenseCategory;
            }
        }
    }
}