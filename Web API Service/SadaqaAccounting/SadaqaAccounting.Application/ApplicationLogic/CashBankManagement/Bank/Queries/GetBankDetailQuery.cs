namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Queries
{
    public class GetBankDetailQuery : IRequest<BankUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetBankDetailQuery, BankUpdateModel>
        {
            private readonly IBankRepository _bankRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IBankRepository bankRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _bankRepository = bankRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<BankUpdateModel> Handle(GetBankDetailQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if bank id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new BankUpdateModel();

                // Decrypt bank id
                var decryptBankId = EncryptionService.Decrypt(request.Id);

                // Check if bank decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptBankId) || string.IsNullOrEmpty(decryptBankId))
                    return new BankUpdateModel();

                // Convert decrypt bank id
                var convertBankId = Convert.ToInt32(decryptBankId);

                var getExistBank = await _bankRepository.GetByIdAsync(convertBankId);

                if (getExistBank is null)
                    return new BankUpdateModel();

                var mapExitBank = _mapper.Map<BankUpdateModel>(getExistBank);
                return mapExitBank;
            }
        }
    }
}