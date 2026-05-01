using SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Model;
using SadaqaAccounting.Repository.Contracts.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Queries
{
    public class GetAllIncomeCategoryQuery: PaginationRequest, IRequest<PaginatedResponse<IncomeCategoryGridModel>>
    {
        public class Handler : IRequestHandler<GetAllIncomeCategoryQuery, PaginatedResponse<IncomeCategoryGridModel>>
        {
            private readonly IIncomeCategoryRepository _IncomeCategoryRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeCategoryRepository _IncomeCategoryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this._IncomeCategoryRepository = _IncomeCategoryRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<IncomeCategoryGridModel>> Handle(GetAllIncomeCategoryQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                var accUnitId = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!;
                if (accUnitId != null)
                {
                    request.AccountUnitId = int.Parse(accUnitId);
                }
                else
                {
                    request.AccountUnitId = 0;
                }

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getIncomeCategories = await _IncomeCategoryRepository.GetIncomeCategoriesFilterAsync(request, cancellationToken);
                var mapIncomeCategories = _mapper.Map<ICollection<IncomeCategoryGridModel>>(getIncomeCategories.Data);

                var result = new PaginatedResponse<IncomeCategoryGridModel>
                {
                    Data = mapIncomeCategories,
                    CurrentPage = getIncomeCategories.CurrentPage,
                    TotalPages = getIncomeCategories.TotalPages,
                    TotalRecords = getIncomeCategories.TotalRecords,
                    PageSize = getIncomeCategories.PageSize
                };

                return result;
            }
        }
    }
}
