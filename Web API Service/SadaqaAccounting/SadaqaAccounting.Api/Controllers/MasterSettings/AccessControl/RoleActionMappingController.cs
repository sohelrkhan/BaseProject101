namespace SadaqaAccounting.Api.Controllers.MasterSettings.AccessControl
{
    public class RoleActionMappingController:BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(RoleActionMappingCreateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<RoleActionMappingCreateModel>> CreateAsync(CreateRoleActionMappingCommand roleActionMappingCommand)
        {
            if (ModelState.IsValid)
            {
                var createRoleAccess = await Mediator.Send(roleActionMappingCommand);
                return Ok(createRoleAccess);
            }

            return BadRequest(roleActionMappingCommand);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleActionMappingGridModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<RoleActionMappingGridModel>> GetRoleById(int id)
        {
            var RoleActionMapping = await Mediator.Send(new GetRoleActionMappingByIdQuery { Id = id });
            return Ok(RoleActionMapping);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<RoleActionMappingGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<RoleActionMappingGridModel>>> GetAllAsync()
        {
            var roleActions = await Mediator.Send(new GetAllRoleActionMappingQuery());
            return Ok(roleActions);
        }
    }
}