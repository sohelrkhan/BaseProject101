namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Command
{
    public class UpdateUserCommand : UserUpdateModel, IRequest<UserUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateUserCommand, UserUpdateModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<UserUpdateModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var existUser = await _userManager.Users.Where(u => u.Id == request.Id).FirstOrDefaultAsync();

                if (existUser is null)
                    throw new BadRequestException(ProvideErrorMessage.UserNotFound);

                existUser = _mapper.Map((UserUpdateModel)request, existUser);
                var result = await _userManager.UpdateAsync(existUser);


                if (result.Succeeded)
                    return request;
                else
                    throw new BadRequestException(ProvideErrorMessage.UserNotFound);
            }
        }
    }
}