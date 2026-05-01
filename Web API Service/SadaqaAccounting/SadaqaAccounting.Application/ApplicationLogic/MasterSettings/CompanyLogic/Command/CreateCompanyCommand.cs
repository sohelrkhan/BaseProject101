namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Command
{
    public class CreateCompanyCommand : CompanyCreateModel, IRequest<CompanyCreateModel>
    {
        public class Handler : IRequestHandler<CreateCompanyCommand, CompanyCreateModel>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IWebHostEnvironment _webHostEnvironment;

            public Handler(ICompanyRepository companyRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, 
                IWebHostEnvironment webHostEnvironment)
            {
                _companyRepository = companyRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _webHostEnvironment = webHostEnvironment;
            }

            public async Task<CompanyCreateModel> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var base64Image = string.Empty;
                var fileName = string.Empty;

                // Check if LogoFile or Logo is provided
                if(request.Logo is not null)
                    fileName = request.Logo;

                // Process the image if provided
                if (!string.IsNullOrEmpty(base64Image) || !string.IsNullOrWhiteSpace(base64Image))
                {
                    var file = ConvertBase64ToIFormFile(base64Image, fileName);
                    var ImagePath = UploadImageProvider.UploadImageFile(_webHostEnvironment, file, "Upload", "CompanyLogo");

                    request.Logo = ImagePath;
                }

                // Map to Company entity
                var createdCompany = _mapper.Map<Company>(request);
                createdCompany.StatusId = GlobalStatus.Active;
                createdCompany.CreatedById = userId;
                createdCompany.CreatedDateTime = DateTime.UtcNow;

                // Create Company
                createdCompany = await _companyRepository.CreateAsync(createdCompany);
                return request;
            }

            // Converts a base64 string to an IFormFile
            public static IFormFile ConvertBase64ToIFormFile(string base64String, string fileName)
            {
                if (string.IsNullOrWhiteSpace(base64String))
                    throw new ArgumentException("Base64 string is null or empty.");

                // Remove the data: image/...; base64, part if it exists
                var base64Parts = Regex.Match(base64String, @"data:(?<type>.+?);base64,(?<data>.+)").Groups;
                string actualData = base64Parts.Count > 0 && base64Parts["data"].Success ? base64Parts["data"].Value : base64String;

                // Decode the base64 string
                byte[] fileBytes = Convert.FromBase64String(actualData);

                // Create a MemoryStream and FormFile
                var stream = new MemoryStream(fileBytes);
                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = GetContentType(fileName)
                };

                return formFile;
            }

            // Determines the content type based on the file extension
            private static string GetContentType(string fileName)
            {
                var extension = Path.GetExtension(fileName).ToLowerInvariant();

                return extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".txt" => "text/plain",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream",
                };
            }
        }
    }
}