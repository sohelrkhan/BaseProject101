using SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Model;
using SadaqaAccounting.Model.Models.IncomeManagement;
using SadaqaAccounting.Repository.Contracts.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Command
{
    public class CreateIncomeCategoryCommand: IncomeCategoryCreateModel, IRequest<IncomeCategoryCreateModel>
    {
        public class Handler : IRequestHandler<CreateIncomeCategoryCommand, IncomeCategoryCreateModel>
        {
            private readonly IIncomeCategoryRepository _IncomeCategoryRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeCategoryRepository IncomeCategoryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _IncomeCategoryRepository = IncomeCategoryRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IncomeCategoryCreateModel> Handle(CreateIncomeCategoryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdIncomeCategory = _mapper.Map<IncomeCategory>(request);
                createdIncomeCategory.AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                createdIncomeCategory.CreatedById = userId;
                createdIncomeCategory.CreatedDateTime = DateTime.UtcNow;
                createdIncomeCategory = await _IncomeCategoryRepository.CreateAsync(createdIncomeCategory);

                return request;
            }
        }
    }
}
