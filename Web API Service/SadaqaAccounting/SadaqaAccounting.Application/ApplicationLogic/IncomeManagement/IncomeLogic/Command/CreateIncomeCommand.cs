namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Command
{
    public class CreateIncomeCommand : IncomeCreateModel, IRequest<IncomeCreateModel>
    {
        public class Handler : IRequestHandler<CreateIncomeCommand, IncomeCreateModel>
        {
            private readonly IIncomeRepository _incomeRepository;
            private readonly ICashLedgerRepository _cashLedgerRepository;
            private readonly IBankLedgerRepository _bankLedgerRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IIncomeRepository incomeRepository,
                ICashLedgerRepository cashLedgerRepository,
                IBankLedgerRepository bankLedgerRepository,
                IUnitOfWork unitOfWork,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor)
            {
                _incomeRepository = incomeRepository;
                _cashLedgerRepository = cashLedgerRepository;
                _bankLedgerRepository = bankLedgerRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IncomeCreateModel> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
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
                    // Create income
                    var createdIncome = _mapper.Map<Income>(request);
                    createdIncome.AccountUnitId = accountUnitId;
                    createdIncome.CreatedById = userId;
                    createdIncome.CreatedDateTime = DateTime.UtcNow;
                    createdIncome.ReceiptDate = DateTime.ParseExact(request.ReceiptDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    createdIncome = await _incomeRepository.CreateAsync(createdIncome);

                    // If payment mode = Cash then insert Cash Ledger table
                    if (request.PaymentModeId == PaymentMode.Cash)
                    {
                        var cashLedger = new CashLedger
                        {
                            AccountUnitId = accountUnitId,
                            TransactionDate = createdIncome.ReceiptDate,
                            TransactionTypeId = TransactionType.In,
                            SourceTypeId = SourceType.Income,
                            SourceId = createdIncome.Id,
                            CashId = (int)request.CashId!,
                            Amount = createdIncome.Amount,
                            CreatedById = createdIncome.CreatedById,
                            CreatedDateTime = createdIncome.CreatedDateTime,
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
                            TransactionDate = createdIncome.ReceiptDate,
                            TransactionTypeId = TransactionType.In,
                            SourceTypeId = SourceType.Income,
                            SourceId = createdIncome.Id,
                            Amount = createdIncome.Amount,
                            CreatedById = createdIncome.CreatedById,
                            CreatedDateTime = createdIncome.CreatedDateTime,
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