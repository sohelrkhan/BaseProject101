namespace SadaqaAccounting.Api.Controllers.MasterSettings.AccessControl
{
    public class UserAccessMappingController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(UserAccessMappingCreateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessMappingCreateModel>> CreateAsync(CreateUserAccessMappingCommand userAccessMappingCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createUserAccess = await Mediator.Send(userAccessMappingCreateModel);
                return Ok(createUserAccess);
            }

            return BadRequest(userAccessMappingCreateModel);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserAccessMappingGridModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessMappingGridModel>> GetUserWiseAccessByIdAsync(string id)
        {
            var userAccess = await Mediator.Send(new GetUserWiseAccessQuery { Id = id });
            return Ok(userAccess);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<UserAccessMappingGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<UserAccessMappingGridModel>>> GetAllAsync()
        {
            var userAccess = await Mediator.Send(new GetAllUserAccessMappingQuery());
            return Ok(userAccess);
        }
    }
}