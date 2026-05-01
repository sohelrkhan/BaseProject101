namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Queries
{
    public class GetCompanyDetailsQuery : IRequest<CompanyUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetCompanyDetailsQuery, CompanyUpdateModel>
        {
            private readonly ICompanyRepository _compnayRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(ICompanyRepository companyRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _compnayRepository = companyRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<CompanyUpdateModel> Handle(GetCompanyDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if company id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new CompanyUpdateModel();

                // Decrypt company id
                var decryptCompanyId = EncryptionService.Decrypt(request.Id);

                // Check if company decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptCompanyId) || string.IsNullOrEmpty(decryptCompanyId))
                    return new CompanyUpdateModel();

                // Convert decrypt company id
                var convertCompanyId = Convert.ToInt32(decryptCompanyId);

                // Get exist company by id
                var getExistCompany = await _compnayRepository.GetByIdAsync(convertCompanyId);

                if (getExistCompany is null)
                    return new CompanyUpdateModel();

                // Map exist company id
                var mapExitCompany = _mapper.Map<CompanyUpdateModel>(getExistCompany);
                mapExitCompany.EncryptedId = EncryptionService.Encrypt(mapExitCompany.Id.ToString());

                return mapExitCompany;
            }
        }
    }
}