namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Command
{
    public class UpdateExpenseCommand : ExpenseUpdateModel, IRequest<ExpenseUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateExpenseCommand, ExpenseUpdateModel>
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

            public async Task<ExpenseUpdateModel> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Get exist expense
                var getExistExpense = await _expenseRepository.GetByIdAsync(request.Id);

                if (getExistExpense is null)
                    throw new BadRequestException(ProvideErrorMessage.ExpenseIdNotFound);

                // Get exist cash ledger by expense id
                var getCashLedger = await _cashLedgerRepository.GetCashLedgerByExpenseAsync(getExistExpense.Id, cancellationToken);

                // Get exist bank ledger by expense id
                var getBankLedger = await _bankLedgerRepository.GetBankLadgetByExpenseAsync(getExistExpense.Id, cancellationToken);

                // Start unit of work
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    getExistExpense = _mapper.Map((ExpenseUpdateModel)request, getExistExpense);
                    getExistExpense.AccountUnitId = accountUnitId;
                    getExistExpense.ExpenseDate = DateTime.ParseExact(request.ExpenseDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    getExistExpense = await _expenseRepository.UpdateAsync(getExistExpense);

                    // If payment mode = Cash then update Cash Ledger table
                    if(getExistExpense.PaymentModeId == PaymentMode.Cash && getCashLedger is not null)
                    {
                        if (getExistExpense.Amount != getCashLedger.Amount)
                        {
                            getCashLedger.Amount = getExistExpense.Amount;
                            await _cashLedgerRepository.UpdateAsync(getCashLedger);
                        }
                    }

                    // If payment mode = Cash then update Cash Ledger table
                    if (getExistExpense.PaymentModeId == PaymentMode.Bank && getBankLedger is not null)
                    {
                        if (getExistExpense.Amount != getBankLedger.Amount)
                        {
                            getBankLedger.Amount = getExistExpense.Amount;
                            await _bankLedgerRepository.UpdateAsync(getBankLedger);
                        }
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