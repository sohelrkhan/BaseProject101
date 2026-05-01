namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Command
{
    public class CreateBankCommand : BankCreateModel, IRequest<BankCreateModel>
    {
        public class Handler : IRequestHandler<CreateBankCommand, BankCreateModel>
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

            public async Task<BankCreateModel> Handle(CreateBankCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check, bank name is exist or not
                var hasBanek = await _bankRepository.HasBankAsync(request.Name);
                if(hasBanek)
                    throw new UnauthorizedAccessException(ProvideErrorMessage.DuplicateBank);

                // Start unit of work
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Create bank
                    var createdBank = _mapper.Map<SadaqaAccounting.Model.Models.Cash_BankManagement.Bank>(request);
                    createdBank.AccountUnitId = accountUnitId;
                    createdBank.CreatedById = userId;
                    createdBank.CreatedDateTime = DateTime.UtcNow;
                    createdBank.StatusId = GlobalStatus.Active;
                    createdBank = await _bankRepository.CreateAsync(createdBank);

                    // Create bank opening balance
                    var bankOpeningBalance = new OpeningBalance
                    {
                        AccountUnitId = accountUnitId,
                        PaymentModeId = PaymentMode.Bank,
                        BankId = createdBank.Id,
                        Amount = createdBank.OpeningBalance,
                        OpeningDate = DateTime.UtcNow,
                        CreatedById = createdBank.CreatedById,
                        CreatedDateTime = DateTime.UtcNow,
                    };

                    await _openingBalanceRepository.CreateAsync(bankOpeningBalance);
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