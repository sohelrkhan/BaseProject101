namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.OpeningBalanceLogic.Queries
{
    public class GetOpeningBalanceDetailQuery : IRequest<OpeningBalanceUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetOpeningBalanceDetailQuery, OpeningBalanceUpdateModel>
        {
            private readonly IOpeningBalanceRepository _openingBalanceRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMapper _mapper;            

            public Handler(
                IOpeningBalanceRepository openingBalanceRepository,
                IHttpContextAccessor httpContextAccessor,
                IMapper mapper)
            {
                _openingBalanceRepository = openingBalanceRepository;
                _httpContextAccessor = httpContextAccessor;
                _mapper = mapper;                
            }

            public async Task<OpeningBalanceUpdateModel> Handle(GetOpeningBalanceDetailQuery request, 
                CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if bank id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new OpeningBalanceUpdateModel();

                // Decrypt opening balance id
                var decryptOpeningBalanceId = EncryptionService.Decrypt(request.Id);

                // Check if bank decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptOpeningBalanceId) || string.IsNullOrEmpty(decryptOpeningBalanceId))
                    return new OpeningBalanceUpdateModel();

                // Convert decrypt opening balance id
                var convertOpeningBalanceId = Convert.ToInt32(decryptOpeningBalanceId);

                var getExistOpeningBalance = await _openingBalanceRepository.GetByIdAsync(convertOpeningBalanceId);

                if (getExistOpeningBalance is null)
                    return new OpeningBalanceUpdateModel();

                var mapExitOpeningBalance = _mapper.Map<OpeningBalanceUpdateModel>(getExistOpeningBalance);
                return mapExitOpeningBalance;
            }
        }
    }
}