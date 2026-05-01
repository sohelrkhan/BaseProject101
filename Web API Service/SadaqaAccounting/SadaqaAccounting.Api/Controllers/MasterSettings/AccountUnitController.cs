using SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Model;

namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class AccountUnitController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<AccountUnitGridModel>), StatusCodes.Status200OK)]
        //[CheckAuthorize("AccountUnit", "List")]
        public async Task<ActionResult<ICollection<AccountUnitGridModel>>> GetAllAsync()
        {
            var getAccountUnits = await Mediator.Send(new GetAllAccountUnitQuery());
            return Ok(getAccountUnits);
        }
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<SelectModel>), StatusCodes.Status200OK)]
        //[CheckAuthorize("AccountUnit", "List")]
        public async Task<ActionResult<ICollection<SelectModel>>> GetSelectListAccountUnitAsync()
        {
            var getAccountUnits = await Mediator.Send(new SelectListAccountUnitQuery());
            return Ok(getAccountUnits);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountUnitViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountUnitViewModel>> GetByIdAsync(int id)
        {
            var accountUnitVm = new AccountUnitViewModel
            {
                UpdateModel = await Mediator.Send(new GetAccountUnitDetailQuery { Id = id })
            };

            return Ok(accountUnitVm);
        }
    }
}