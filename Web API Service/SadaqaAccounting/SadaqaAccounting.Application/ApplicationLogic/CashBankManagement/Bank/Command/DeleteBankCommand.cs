namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Command
{
    public class DeleteBankCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteBankCommand, bool>
        {
            private readonly IBankRepository _bankRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IBankRepository bankRepository, IHttpContextAccessor httpContextAccessor)
            {
                _bankRepository = bankRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDelete = false;
                var existBank = await _bankRepository.GetByIdAsync(request.Id);

                if (existBank is null)
                    throw new BadRequestException(ProvideErrorMessage.BankIdNotFound);

                if (existBank is not null)
                {
                    existBank.IsDeleted = true;
                    existBank.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _bankRepository.UpdateAsync(existBank);
                    isDelete = true;
                }

                return isDelete;
            }
        }
    }
}