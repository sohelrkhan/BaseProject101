namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Queries
{
    public class SelectListUserQuery : IRequest<IEnumerable<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListUserQuery, IEnumerable<SelectModel>>
        {
            private readonly UserManager<User> _userManager;
            public Handler(UserManager<User> userManager, IMapper mapper) => _userManager = userManager;

            public async Task<IEnumerable<SelectModel>> Handle(SelectListUserQuery request, CancellationToken cancellationToken)
            {
                var users = _userManager.Users
                    .AsNoTracking()
                    .Select(s => new SelectModel
                    {
                        Id = s.Id,
                        Name = s.FullName,                                
                    });
                
                return await users.ToListAsync();
            }
        }
    }
}
