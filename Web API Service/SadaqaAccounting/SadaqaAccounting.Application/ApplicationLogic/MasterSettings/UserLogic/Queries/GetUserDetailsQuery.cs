namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Queries
{
    public class GetUserDetailsQuery:IRequest<UserUpdateModel>
    {
        public string Id { get; set; }
        public class Handler : IRequestHandler<GetUserDetailsQuery, UserUpdateModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }
            public async Task<UserUpdateModel> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                var mapUser = _mapper.Map<UserUpdateModel>(user);
                return mapUser;
            }
        }
    }
}
