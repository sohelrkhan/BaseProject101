namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Command
{
    public class CreateExpenseCommand : ExpenseCreateModel, IRequest<ExpenseCreateModel>
    {
        public class Handler : IRequestHandler<CreateExpenseCommand, ExpenseCreateModel>
        {
            private readonly IExpenseRepository _expenseRepository;
            private readonly ICashLedgerRepository _cashLedgerRepository;
            private readonly IBankLedgerRepository _bankLedgerRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IExpenseRepository expenseRepository, 
                ICashLedgerRepository cashLedgerRepository, 
                IBankLedgerRepository bankLedgerRepository, 
                IUnitOfWork unitOfWork,
                IMapper mapper, 
                IHttpContextAccessor httpContextAccessor)
            {
                _expenseRepository = expenseRepository;
                _cashLedgerRepository = cashLedgerRepository;
                _bankLedgerRepository = bankLedgerRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ExpenseCreateModel> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Start unit of work
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Create expense
                    var createdExpense = _mapper.Map<Expense>(request);
                    createdExpense.AccountUnitId = accountUnitId;
                    createdExpense.CreatedById = userId;
                    createdExpense.CreatedDateTime = DateTime.UtcNow;
                    createdExpense.ExpenseDate = DateTime.ParseExact(request.ExpenseDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    createdExpense = await _expenseRepository.CreateAsync(createdExpense);

                    // If payment mode = Cash then insert Cash Ledger table
                    if (request.PaymentModeId == PaymentMode.Cash)
                    {
                        var cashLedger = new CashLedger
                        {
                            AccountUnitId = accountUnitId,
                            TransactionDate = createdExpense.ExpenseDate,
                            TransactionTypeId = TransactionType.Out,
                            SourceTypeId = SourceType.Expense,
                            SourceId = createdExpense.Id,
                            CashId = (int)request.CashId!,
                            Amount = createdExpense.Amount,
                            CreatedById = createdExpense.CreatedById,
                            CreatedDateTime = createdExpense.CreatedDateTime,
                        };

                        await _cashLedgerRepository.CreateAsync(cashLedger);
                    }

                    // If payment mode = Bank then insert Bank Ledger table
                    if (request.PaymentModeId == PaymentMode.Bank)
                    {
                        var bankLedger = new BankLedger
                        {
                            AccountUnitId = accountUnitId,
                            BankId = (int)request.BankId!,
                            TransactionDate = createdExpense.ExpenseDate,
                            TransactionTypeId = TransactionType.Out,
                            SourceTypeId = SourceType.Expense,
                            SourceId = createdExpense.Id,
                            Amount = createdExpense.Amount,
                            CreatedById = createdExpense.CreatedById,
                            CreatedDateTime = createdExpense.CreatedDateTime,
                        };

                        await _bankLedgerRepository.CreateAsync(bankLedger);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return request;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }
    }
}