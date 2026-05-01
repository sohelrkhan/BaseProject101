namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.Command
{
    public class ChangePasswordCommand: ChangePassword, IRequest<UserModel>
    {
        public class Handler : IRequestHandler<ChangePasswordCommand, UserModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<UserModel> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var existUser = await _userManager.FindByEmailAsync(request.Email);
                if (existUser == null)
                    throw new Exception("User not found");

                var removeResult = await _userManager.RemovePasswordAsync(existUser);
                if (!removeResult.Succeeded)
                    throw new Exception("Failed to reset password.");

                var addResult = await _userManager.AddPasswordAsync(existUser, request.ConfirmPassword);
                if (!addResult.Succeeded)
                    throw new Exception("Failed to add new password.");

                existUser.ForcePasswordChanged = request.ForcePasswordChanged;
                await _userManager.UpdateAsync(existUser);

                var mapUser = _mapper.Map<UserModel>(existUser);
                return mapUser;
            }
        }
    }
}