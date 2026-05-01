namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Queries
{
    public class GetEmployeeDetailsQuery : IRequest<EmployeeUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetEmployeeDetailsQuery, EmployeeUpdateModel>
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

            public async Task<EmployeeUpdateModel> Handle(GetEmployeeDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if employee id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new EmployeeUpdateModel();

                // Decrypt employee id
                var decryptEmployeeId = EncryptionService.Decrypt(request.Id);

                // Check if employee decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptEmployeeId) || string.IsNullOrEmpty(decryptEmployeeId))
                    return new EmployeeUpdateModel();

                // Convert decrypt employee id
                var convertEmployeeId = Convert.ToInt32(decryptEmployeeId);

                var getExistEmployee = await _employeeRepository.GetByIdAsync(convertEmployeeId);

                if (getExistEmployee is null)
                    return new EmployeeUpdateModel();

                var mapExitEmployee = _mapper.Map<EmployeeUpdateModel>(getExistEmployee);
                return mapExitEmployee;
            }
        }
    }
}