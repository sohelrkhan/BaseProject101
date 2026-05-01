namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Command
{
    public class CompanylogoUpdateCommand : CompanylogoUpdateModel, IRequest<CompanylogoUpdateModel>
    {
        public class Handler : IRequestHandler<CompanylogoUpdateCommand, CompanylogoUpdateModel>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IWebHostEnvironment _webHostEnvironment;

            public Handler(ICompanyRepository companyRepository, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
            {
                _companyRepository = companyRepository;
                _httpContextAccessor = httpContextAccessor;
                _webHostEnvironment = webHostEnvironment;
            }

            public async Task<CompanylogoUpdateModel> Handle(CompanylogoUpdateCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getExistcompany = await _companyRepository.GetByIdAsync(request.Id);

                if (getExistcompany is null)
                    throw new BadRequestException(ProvideErrorMessage.CompanyIdNotFound);

                var ImagePath = UploadImageProvider.UploadImageFile(_webHostEnvironment, request.ImageFile!, "Upload", "companylogo");

                getExistcompany.Logo = ImagePath;
                getExistcompany.UpdatedById = userId;
                getExistcompany.UpdatedDateTime = DateTime.UtcNow;

                getExistcompany = await _companyRepository.UpdateAsync(getExistcompany);
                return request;
            }
        }
    }
}