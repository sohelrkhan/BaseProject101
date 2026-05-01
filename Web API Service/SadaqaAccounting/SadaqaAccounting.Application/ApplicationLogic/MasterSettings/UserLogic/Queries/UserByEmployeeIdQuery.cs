namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Queries
{
    public class UserByEmployeeIdQuery : IRequest<bool>
    {
        public int? EmployeeId { get; set; }

        public class Handler : IRequestHandler<UserByEmployeeIdQuery, bool>
        {
            private readonly UserManager<User> _userManager;
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper, IEmployeeRepository employeeRepository)
            {
                _userManager = userManager;
                _employeeRepository = employeeRepository;
                _mapper = mapper;
            }

            public async Task<bool> Handle(UserByEmployeeIdQuery request, CancellationToken cancellationToken)
            {
                if (request.EmployeeId is null)
                    return false;

                var users = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
                return users.Any(u => u.EmployeeId == request.EmployeeId);
            }
        }
    }
}