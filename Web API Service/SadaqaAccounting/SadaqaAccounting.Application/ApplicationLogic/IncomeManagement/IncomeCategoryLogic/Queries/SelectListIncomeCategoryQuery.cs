using SadaqaAccounting.Repository.Contracts.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Queries
{
    public class SelectListIncomeCategoryQuery: IRequest<List<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListIncomeCategoryQuery, List<SelectModel>>
        {
            private readonly IIncomeCategoryRepository _IncomeCategoryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeCategoryRepository IncomeCategoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                _IncomeCategoryRepository = IncomeCategoryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<List<SelectModel>> Handle(SelectListIncomeCategoryQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getIncomeCategories = await _IncomeCategoryRepository.GetIncomeCategorySelectListAsync(AccountUnitId);

                var IncomeCategorySelectList = getIncomeCategories
                    .Select(s => new SelectModel()
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();

                return IncomeCategorySelectList;
            }
        }
    }
}
