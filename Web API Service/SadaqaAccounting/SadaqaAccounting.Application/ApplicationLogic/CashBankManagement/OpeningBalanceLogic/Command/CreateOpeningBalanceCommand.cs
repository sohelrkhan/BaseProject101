namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.OpeningBalanceLogic.Command
{
    public class CreateOpeningBalanceCommand : OpeningBalanceCreateModel, IRequest<OpeningBalanceCreateModel>
    {
        public class Handler : IRequestHandler<CreateOpeningBalanceCommand, OpeningBalanceCreateModel>
        {
            private readonly IOpeningBalanceRepository _openingBalanceRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IOpeningBalanceRepository openingBalanceRepository,
                IUnitOfWork unitOfWork,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor)
            {
                _openingBalanceRepository = openingBalanceRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<OpeningBalanceCreateModel> Handle(CreateOpeningBalanceCommand request, 
                CancellationToken cancellationToken)
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
                    // Create opening balance
                    var createdOpeningBalance = _mapper.Map<OpeningBalance>(request);
                    createdOpeningBalance.AccountUnitId = accountUnitId;
                    createdOpeningBalance.CreatedById = userId;
                    createdOpeningBalance.CreatedDateTime = DateTime.UtcNow;

                    // Save opening balance to database
                    await _openingBalanceRepository.CreateAsync(createdOpeningBalance);
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