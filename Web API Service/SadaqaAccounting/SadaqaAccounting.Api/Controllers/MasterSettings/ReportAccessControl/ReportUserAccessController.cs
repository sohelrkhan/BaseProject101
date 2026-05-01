namespace SadaqaAccounting.Api.Controllers.MasterSettings.ReportAccessControl
{
    public class ReportUserAccessController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ReportUserAccessGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<ReportUserAccessGridModel>>> GetAllAsync()
        {
            var getReportUserAccesses = await Mediator.Send(new GetAllReportUserAccessQuery());
            return Ok(getReportUserAccesses);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ReportUserAccessGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<ReportUserAccessGridModel>>> GetAllReportUserAccessGroupByUserIdAsync()
        {
            var getReportUserAccesses = await Mediator.Send(new GetAllReportUserAccessGroupByUserIdQuery());
            return Ok(getReportUserAccesses);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReportUserAccessCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("ReportUserAccess", "List")]
        public async Task<ActionResult<ReportUserAccessCreateModel>> CreateAsync(CreateReportUserAccessCommand reportUserAccessCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createReportUserAccess = await Mediator.Send(reportUserAccessCreateModel);
                return Ok(createReportUserAccess);
            }
            return BadRequest(reportUserAccessCreateModel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ReportRegistryUserAccessGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<ReportRegistryUserAccessGridModel>>> GetAllReportRegistryAsync()
        {
            var getReportRegistries = await Mediator.Send(new GetAllReportRegistryUserAccessQuery());
            return Ok(getReportRegistries);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ICollection<ReportRegistryUserAccessGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<ReportRegistryUserAccessGridModel>>> GetReportUserAccessesByUserAsync(string userId)
        {
            var reportUserAccess = await Mediator.Send(new GetReportUserAccessesByUserQuery { UserId = userId });
            return Ok(reportUserAccess);
        }
    }
}