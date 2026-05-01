using SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Model;
using SadaqaAccounting.Repository.Contracts.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Queries
{
    public class GetIncomeCategoryDetailsQuery: IRequest<IncomeCategoryUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetIncomeCategoryDetailsQuery, IncomeCategoryUpdateModel>
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

            public async Task<IncomeCategoryUpdateModel> Handle(GetIncomeCategoryDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if feature id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new IncomeCategoryUpdateModel();

                // Decrypt Income category id
                var decryptIncomeCategoryId = EncryptionService.Decrypt(request.Id);

                // Check if Income category decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptIncomeCategoryId) || string.IsNullOrEmpty(decryptIncomeCategoryId))
                    return new IncomeCategoryUpdateModel();

                // Convert decrypt Income category id
                var convertIncomeCategoryId = Convert.ToInt32(decryptIncomeCategoryId);

                var getExistIncomeCategory = await _IncomeCategoryRepository.GetByIdAsync(convertIncomeCategoryId);

                if (getExistIncomeCategory is null)
                    return new IncomeCategoryUpdateModel();

                var mapExitIncomeCategory = _mapper.Map<IncomeCategoryUpdateModel>(getExistIncomeCategory);
                return mapExitIncomeCategory;
            }
        }
    }
}
