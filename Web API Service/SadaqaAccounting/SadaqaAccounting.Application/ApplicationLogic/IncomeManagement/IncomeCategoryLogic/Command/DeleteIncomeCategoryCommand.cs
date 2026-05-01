using SadaqaAccounting.Repository.Contracts.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Command
{
    public class DeleteIncomeCategoryCommand: IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteIncomeCategoryCommand, bool>
        {
            private readonly IIncomeCategoryRepository _IncomeCategoryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeCategoryRepository IncomeCategoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _IncomeCategoryRepository = IncomeCategoryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteIncomeCategoryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDeleteAction = false;
                var existIncomeCategory = await _IncomeCategoryRepository.GetByIdAsync(request.Id);

                if (existIncomeCategory is null)
                    throw new BadRequestException(ProvideErrorMessage.IncomeCategoryIdNotFound);

                if (existIncomeCategory is not null)
                {
                    existIncomeCategory.IsDeleted = true;
                    existIncomeCategory.DeletedDateTime = DateTime.UtcNow;

                    var updatedAction = await _IncomeCategoryRepository.UpdateAsync(existIncomeCategory);
                    isDeleteAction = true;
                }

                return isDeleteAction;
            }
        }
    }
}
