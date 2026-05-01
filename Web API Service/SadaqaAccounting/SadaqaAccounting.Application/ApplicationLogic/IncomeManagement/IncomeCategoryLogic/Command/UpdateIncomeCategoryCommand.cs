using SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Model;
using SadaqaAccounting.Repository.Contracts.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Command
{
    public class UpdateIncomeCategoryCommand: IncomeCategoryUpdateModel, IRequest<IncomeCategoryUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateIncomeCategoryCommand, IncomeCategoryUpdateModel>
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

            public async Task<IncomeCategoryUpdateModel> Handle(UpdateIncomeCategoryCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist Income category
                var getExistIncomeCategory = await _IncomeCategoryRepository.GetByIdAsync(request.Id);

                if (getExistIncomeCategory is null)
                    throw new BadRequestException(ProvideErrorMessage.IncomeCategoryIdNotFound);

                getExistIncomeCategory = _mapper.Map((IncomeCategoryUpdateModel)request, getExistIncomeCategory);
                getExistIncomeCategory.AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                getExistIncomeCategory = await _IncomeCategoryRepository.UpdateAsync(getExistIncomeCategory);

                return request;
            }
        }
    }
}
