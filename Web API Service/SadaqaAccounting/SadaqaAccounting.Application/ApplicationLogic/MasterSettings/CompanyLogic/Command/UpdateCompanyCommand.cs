namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Command
{
    public class UpdateCompanyCommand : CompanyUpdateModel, IRequest<CompanyUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateCompanyCommand, CompanyUpdateModel>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(ICompanyRepository companyRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
            {
                _companyRepository = companyRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _unitOfWork = unitOfWork;
            }

            public async Task<CompanyUpdateModel> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Decrypt company id
                var decryptCompanyId = EncryptionService.Decrypt(request.EncryptedId);

                // Convert decrypt company id
                var convertCompanyId = Convert.ToInt32(decryptCompanyId);

                // Get exist company
                var getExistCompany = await _companyRepository.GetByIdAsync(convertCompanyId);

                if (getExistCompany is null)
                    throw new BadRequestException(ProvideErrorMessage.CompanyNotFound);

                // Map update model to entity
                getExistCompany = _mapper.Map((CompanyUpdateModel)request, getExistCompany);
                getExistCompany.UpdatedById = userId;
                getExistCompany.UpdatedDateTime = DateTime.UtcNow;

                // Update company
                getExistCompany = await _companyRepository.UpdateAsync(getExistCompany);
                return request;
            }
        }
    }
}