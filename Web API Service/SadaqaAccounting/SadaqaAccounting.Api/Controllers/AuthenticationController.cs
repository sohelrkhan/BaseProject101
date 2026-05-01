namespace SadaqaAccounting.Api.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        [HttpPost]
        public async Task<ICollection<UserModel>> GetAllUsersAsync(int companyId)
        {
            var users = await Mediator.Send(new GetUsersCommand { CompanyId = companyId }); 
            return users;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdentityViewModel), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IdentityViewModel>> Login(LoginCommand command)
        {
            var ivm = new IdentityViewModel()
            {
                UserModel = await Mediator.Send(command)
            };

            return Ok(ivm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdentityViewModel), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IdentityViewModel>> ChnagePassword(ChangePasswordCommand command)
        {
            var ivm = new IdentityViewModel()
            {
                UserModel = await Mediator.Send(command)
            };

            return Ok(ivm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ResetForceChangePasswords(string[] Emails)
        {
            var resetForcePassword = await Mediator.Send(new ResetPasswordsForReLoginAgainCommand { Emails = Emails });
            return Ok(resetForcePassword);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserModel>> Registration(RegistrationCommand command)
        {
            var registerUser = await Mediator.Send(command);
            return Ok(registerUser);
        }

        [Authorize]
        [HttpGet("{accountUnitId}")]
        public async Task<ActionResult<IdentityViewModel>> SetLoginUserAccountUnit(int accountUnitId)
        {
            var updatedUser = await Mediator.Send(new SelectAccountUnitCommand { AccountUnitId = accountUnitId });
            return Ok(new IdentityViewModel { UserModel = updatedUser });
        }
    }
}