namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Command
{
    public class DeleteIncomeCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteIncomeCommand, bool>
        {
            private readonly IIncomeRepository _incomeRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeRepository incomeRepository, IHttpContextAccessor httpContextAccessor)
            {
                _incomeRepository = incomeRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDeleteAction = false;
                var existincome = await _incomeRepository.GetByIdAsync(request.Id);

                if (existincome is null)
                    throw new BadRequestException(ProvideErrorMessage.IncomeIdNotFound);

                if (existincome is not null)
                {
                    existincome.IsDeleted = true;
                    existincome.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _incomeRepository.UpdateAsync(existincome);
                    isDeleteAction = true;
                }

                return isDeleteAction;
            }
        }
    }
}