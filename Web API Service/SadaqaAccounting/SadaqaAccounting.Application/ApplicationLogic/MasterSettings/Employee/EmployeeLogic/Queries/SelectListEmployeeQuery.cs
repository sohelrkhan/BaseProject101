public class SelectListEmployeeQuery : IRequest<ICollection<SelectModel>>
{
    public class Handler : IRequestHandler<SelectListEmployeeQuery, ICollection<SelectModel>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessors)
        {
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessors;
        }

        public async Task<ICollection<SelectModel>> Handle(SelectListEmployeeQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the user's Id from the current HTTP context
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            // Check if the user Id is null or not
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

            var employeeList = await _employeeRepository.EmployeeSelectListAsync(cancellationToken);
            return employeeList;
        }
    }
}