namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Command
{
    public class UpdateIncomeCommand : IncomeUpdateModel, IRequest<IncomeUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateIncomeCommand, IncomeUpdateModel>
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

            public async Task<IncomeUpdateModel> Handle(UpdateIncomeCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Get exist income
                var getExistIncome = await _incomeRepository.GetByIdAsync(request.Id);

                if (getExistIncome is null)
                    throw new BadRequestException(ProvideErrorMessage.IncomeIdNotFound);

                // Get exist cash ledger by income id
                var getCashLedger = await _cashLedgerRepository.GetCashLedgerByIncomeAsync(getExistIncome.Id, cancellationToken);

                // Get exist bank ledger by income id
                var getBankLedger = await _bankLedgerRepository.GetBankLadgetByIncomeAsync(getExistIncome.Id, cancellationToken);

                // Start unit of work
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    getExistIncome = _mapper.Map((IncomeUpdateModel)request, getExistIncome);
                    getExistIncome.AccountUnitId = accountUnitId;
                    getExistIncome.ReceiptDate = DateTime.ParseExact(request.ReceiptDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    getExistIncome = await _incomeRepository.UpdateAsync(getExistIncome);

                    // If payment mode = Cash then update Cash Ledger table
                    if (getExistIncome.PaymentModeId == PaymentMode.Cash && getCashLedger is not null)
                    {
                        if (getExistIncome.Amount != getCashLedger.Amount)
                        {
                            getCashLedger.Amount = getExistIncome.Amount;
                            await _cashLedgerRepository.UpdateAsync(getCashLedger);
                        }
                    }

                    // If payment mode = Cash then update Cash Ledger table
                    if (getExistIncome.PaymentModeId == PaymentMode.Bank && getBankLedger is not null)
                    {
                        if (getExistIncome.Amount != getBankLedger.Amount)
                        {
                            getBankLedger.Amount = getExistIncome.Amount;
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