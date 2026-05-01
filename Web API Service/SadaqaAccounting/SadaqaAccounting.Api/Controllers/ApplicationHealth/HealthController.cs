namespace SadaqaAccounting.Api.Controllers.ApplicationHealth
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Healthy");
    }
}