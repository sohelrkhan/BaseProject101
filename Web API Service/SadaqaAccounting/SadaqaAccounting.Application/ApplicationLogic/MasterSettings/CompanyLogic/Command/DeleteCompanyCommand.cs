namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Command
{
    public class DeleteCompanyCommand : IRequest<bool>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteCompanyCommand, bool>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

            public Handler(ICompanyRepository companyRepository, IHttpContextAccessor httpContextAccessor)
            {
                _companyRepository = companyRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Decrypt company id
                var decryptCompanyId = EncryptionService.Decrypt(request.Id);

                // Convert decrypt company id
                var convertCompanyId = Convert.ToInt32(decryptCompanyId);

                var isCompanyDelete = false;
                var existCompany = await _companyRepository.GetByIdAsync(convertCompanyId);

                if (existCompany is null)
                    throw new BadRequestException(ProvideErrorMessage.CompanyIdNotFound);

                if (existCompany is not null)
                {
                    existCompany.IsDeleted = true;
                    existCompany.DeletedDateTime = DateTime.UtcNow;

                    var updatedCompany = await _companyRepository.UpdateAsync(existCompany);
                    isCompanyDelete = true;
                }

                return isCompanyDelete;
            }
        }
    }
}