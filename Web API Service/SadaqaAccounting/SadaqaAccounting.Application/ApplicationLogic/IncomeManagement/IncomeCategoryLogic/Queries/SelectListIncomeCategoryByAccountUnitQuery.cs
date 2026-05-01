namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Queries
{
    public class SelectListIncomeCategoryByAccountUnitQuery : IRequest<IEnumerable<SelectModel>>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectListIncomeCategoryByAccountUnitQuery, IEnumerable<SelectModel>>
        {
            private readonly IIncomeCategoryRepository _incomeCategoryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeCategoryRepository incomeCategoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _incomeCategoryRepository = incomeCategoryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListIncomeCategoryByAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getIncomeCategories = await _incomeCategoryRepository.GetIncomeCategorySelectListByAccountUnitAsync(request.AccountUnitId, cancellationToken);
                return getIncomeCategories;
            }
        }
    }
}