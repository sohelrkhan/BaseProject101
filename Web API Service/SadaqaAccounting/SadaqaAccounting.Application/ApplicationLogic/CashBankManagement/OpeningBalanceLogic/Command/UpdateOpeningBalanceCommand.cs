namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.OpeningBalanceLogic.Command
{
    public class UpdateOpeningBalanceCommand : OpeningBalanceUpdateModel, IRequest<OpeningBalanceUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateOpeningBalanceCommand, OpeningBalanceUpdateModel>
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

            public async Task<OpeningBalanceUpdateModel> Handle(UpdateOpeningBalanceCommand request, 
                CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get login user account unit id
                var accountUnitIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;
                var accountUnitId = int.TryParse(accountUnitIdStr, out var v) ? v : 0;

                // Get exist opening balance by id
                var getExistOpeningBalance = await _openingBalanceRepository.GetByIdAsync(request.Id);

                if (getExistOpeningBalance is null)
                    throw new BadRequestException(ProvideErrorMessage.OpeningBalanceIdNotFound);

                // Start unit of work
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    getExistOpeningBalance = _mapper.Map((OpeningBalanceUpdateModel)request, getExistOpeningBalance);
                    getExistOpeningBalance.AccountUnitId = accountUnitId;
                    getExistOpeningBalance.UpdatedById = userId;
                    getExistOpeningBalance.UpdatedDateTime = DateTime.UtcNow;
                    getExistOpeningBalance = await _openingBalanceRepository.UpdateAsync(getExistOpeningBalance);

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