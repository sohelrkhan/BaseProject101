namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Command
{
    public class UpdateBankCommand : BankUpdateModel, IRequest<BankUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateBankCommand, BankUpdateModel>
        {
            private readonly IBankRepository _bankRepository;
            private readonly IOpeningBalanceRepository _openingBalanceRepository;
            private readonly IUnitOfWork _unitOfWork;   
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IBankRepository bankRepository, 
                IOpeningBalanceRepository openingBalanceRepository, 
                IUnitOfWork unitOfWork, 
                IMapper mapper, 
                IHttpContextAccessor httpContextAccessor)
            {
                _bankRepository = bankRepository;
                _openingBalanceRepository = openingBalanceRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<BankUpdateModel> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Get exist bank
                var getExistBank = await _bankRepository.GetByIdAsync(request.Id);

                if (getExistBank is null)
                    throw new BadRequestException(ProvideErrorMessage.ExpenseIdNotFound);

                // Get exist opening balance by bank id
                var existOpeningBalance = await _openingBalanceRepository.GetOpeningBalanceByBankAsync(getExistBank.Id, cancellationToken);

                // Start unit of work
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    getExistBank = _mapper.Map((BankUpdateModel)request, getExistBank);
                    getExistBank.AccountUnitId = accountUnitId;

                    getExistBank = await _bankRepository.UpdateAsync(getExistBank);

                    // Check bank opening balance amount & Opening Balance table amount are same
                    if(existOpeningBalance is not null && getExistBank.OpeningBalance != existOpeningBalance.Amount)
                    {
                        existOpeningBalance.Amount = getExistBank.OpeningBalance;
                        await _openingBalanceRepository.UpdateAsync(existOpeningBalance);
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