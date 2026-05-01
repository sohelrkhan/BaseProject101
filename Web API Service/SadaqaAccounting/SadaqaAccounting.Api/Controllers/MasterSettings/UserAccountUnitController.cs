namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class UserAccountUnitController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(UserAccountUnitCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("UserAccountUnit", "Create")]
        public async Task<ActionResult<UserAccountUnitCreateModel>> CreateAsync(CreateUserAccountUnitCommand userAccountUnitCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createUserAccountUnit = await Mediator.Send(userAccountUnitCreateModel);
                return Ok(createUserAccountUnit);
            }
            return BadRequest(userAccountUnitCreateModel);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserAccountUnitGridModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccountUnitGridModel>> GetUserAccountUnitAsync(string userId)
        {
            var userAccountUnits = await Mediator.Send(new GetUserAccountUnitQuery { UserId = userId });
            return Ok(userAccountUnits);
        }
    }
}