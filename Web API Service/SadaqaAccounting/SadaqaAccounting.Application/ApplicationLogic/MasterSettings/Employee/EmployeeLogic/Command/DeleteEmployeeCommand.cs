namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Command
{
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteEmployeeCommand, bool>
        {
            private readonly IEmployeeRepository _employeeRepository;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEmployeeRepository employeeRepository, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _employeeRepository = employeeRepository;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Decrypt employee id
                var decryptEmployeeId = EncryptionService.Decrypt(request.Id);

                // Convert decrypt employee id
                var convertEmployeeId = Convert.ToInt32(decryptEmployeeId);

                var isDeleteEmployee = false;
                var existEmployee = await _employeeRepository.GetByIdAsync(convertEmployeeId);

                if (existEmployee is null)
                    throw new BadRequestException(ProvideErrorMessage.EmployeeIdNotFound);

                if (existEmployee is not null)
                {
                    existEmployee.IsDeleted = true;
                    existEmployee.DeletedDateTime = DateTime.UtcNow;
                    var updatedEmployee = await _employeeRepository.UpdateAsync(existEmployee);

                    // Remove employee user table also
                    var getEmployeeAsUser = await _userManager.FindByEmailAsync(existEmployee.Email!);
                    await _userManager.DeleteAsync(getEmployeeAsUser);

                    isDeleteEmployee = true;
                }

                return isDeleteEmployee;
            }
        }
    }
}