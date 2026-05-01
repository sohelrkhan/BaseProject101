namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Queries
{
    public class GetIncomeDetailQuery : IRequest<IncomeUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetIncomeDetailQuery, IncomeUpdateModel>
        {
            private readonly IIncomeRepository _incomeRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IIncomeRepository incomeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _incomeRepository = incomeRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IncomeUpdateModel> Handle(GetIncomeDetailQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if income id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new IncomeUpdateModel();

                // Decrypt income id
                var decryptIncomeId = EncryptionService.Decrypt(request.Id);

                // Check if income decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptIncomeId) || string.IsNullOrEmpty(decryptIncomeId))
                    return new IncomeUpdateModel();

                // Convert decrypt income id
                var convertIncomeId = Convert.ToInt32(decryptIncomeId);

                var getExistIncome = await _incomeRepository.GetByIdAsync(convertIncomeId);

                if (getExistIncome is null)
                    return new IncomeUpdateModel();

                var mapExitIncome = _mapper.Map<IncomeUpdateModel>(getExistIncome);
                mapExitIncome.ReceiptDateString = Convert.ToDateTime(getExistIncome.ReceiptDate).ToString("dd-MM-yyyy");

                return mapExitIncome;
            }
        }
    }
}