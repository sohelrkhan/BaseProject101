namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.Command
{
    public class ResetPasswordsForReLoginAgainCommand : IRequest<bool>
    {
        public string[] Emails { get; set; }

        public class Handler : IRequestHandler<ResetPasswordsForReLoginAgainCommand, bool>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<bool> Handle(ResetPasswordsForReLoginAgainCommand request, CancellationToken cancellationToken)
            {
                var tasks = request.Emails.Select(async email =>
                {
                    var existUser = await _userManager.FindByEmailAsync(email);
                    if (existUser == null)
                    {
                        return false;
                    }

                    existUser.ForcePasswordChanged = false;
                    var result = await _userManager.UpdateAsync(existUser);

                    if (!result.Succeeded)
                    {
                        return false;
                    }

                    return true;
                });

                var results = await Task.WhenAll(tasks);

                return results.All(r => r);
            }
        }
    }
}