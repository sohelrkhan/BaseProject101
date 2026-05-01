namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.OpeningBalanceLogic.Command
{
    public class DeleteOpeningBalanceCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteOpeningBalanceCommand, bool>
        {
            private readonly IOpeningBalanceRepository _openingBalanceRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IOpeningBalanceRepository openingBalanceRepository, 
                IHttpContextAccessor httpContextAccessor)
            {
                _openingBalanceRepository = openingBalanceRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteOpeningBalanceCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDelete = false;
                var existOpeningBalance = await _openingBalanceRepository.GetByIdAsync(request.Id);

                if (existOpeningBalance is null)
                    throw new BadRequestException(ProvideErrorMessage.OpeningBalanceIdNotFound);

                if (existOpeningBalance is not null)
                {
                    existOpeningBalance.IsDeleted = true;
                    existOpeningBalance.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _openingBalanceRepository.UpdateAsync(existOpeningBalance);
                    isDelete = true;
                }

                return isDelete;
            }
        }
    }
}