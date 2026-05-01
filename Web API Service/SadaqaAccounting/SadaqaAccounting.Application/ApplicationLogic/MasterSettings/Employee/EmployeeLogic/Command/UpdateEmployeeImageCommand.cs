namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Command
{
    public class UpdateEmployeeImageCommand : EmployeeImageUpdateModel, IRequest<EmployeeImageUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateEmployeeImageCommand, EmployeeImageUpdateModel>
        {
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IWebHostEnvironment _webHostEnvironment;

            public Handler(IEmployeeRepository employeeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
            {
                _employeeRepository = employeeRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _webHostEnvironment = webHostEnvironment;
            }

            public async Task<EmployeeImageUpdateModel> Handle(UpdateEmployeeImageCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist employee
                var getExistEmployee = await _employeeRepository.GetByIdAsync(request.Id);

                if (getExistEmployee is null)
                    throw new BadRequestException(ProvideErrorMessage.EmployeeIdNotFound);

                var employeeImagePath = UploadImageProvider.UploadImageFile(_webHostEnvironment, request.ImageFile!, "Upload", "EmployeeImage");

                getExistEmployee.Image = employeeImagePath;
                getExistEmployee.UpdatedById = userId;
                getExistEmployee.UpdatedDateTime = DateTime.UtcNow;

                getExistEmployee = await _employeeRepository.UpdateAsync(getExistEmployee);
                return request;
            }
        }
    }
}