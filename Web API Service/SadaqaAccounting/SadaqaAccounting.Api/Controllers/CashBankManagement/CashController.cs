namespace SadaqaAccounting.Api.Controllers.CashBankManagement
{
    public class CashController : BaseController
    {
        [HttpGet("{accountUnitId}")]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListCashByAccountUnitAsync(int accountUnitId)
        {
            var cashes = await Mediator.Send(new SelectListCashByAccountUnitQuery { AccountUnitId = accountUnitId });
            return Ok(cashes);
        }
    }
}