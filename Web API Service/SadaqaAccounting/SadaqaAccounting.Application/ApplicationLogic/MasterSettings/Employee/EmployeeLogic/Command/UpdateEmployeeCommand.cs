namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Command
{
    public class UpdateEmployeeCommand : EmployeeUpdateModel, IRequest<EmployeeUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateEmployeeCommand, EmployeeUpdateModel>
        {
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEmployeeRepository employeeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _employeeRepository = employeeRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<EmployeeUpdateModel> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
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

                getExistEmployee = _mapper.Map((EmployeeUpdateModel)request, getExistEmployee);
                getExistEmployee.UpdatedById = userId;
                getExistEmployee.UpdatedDateTime = DateTime.UtcNow;

                getExistEmployee = await _employeeRepository.UpdateAsync(getExistEmployee);
                return request;
            }
        }
    }
}