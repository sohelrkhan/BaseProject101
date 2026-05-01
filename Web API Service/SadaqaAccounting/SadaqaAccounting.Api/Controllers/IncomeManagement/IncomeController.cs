namespace SadaqaAccounting.Api.Controllers.IncomeManagement
{
    public class IncomeController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<IncomeGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Income", "List")]
        public async Task<ActionResult<PaginatedResponse<IncomeGridModel>>> GetAllAsync(GetAllIncomeQuery getAllIncomeQuery)
        {
            var getIncomes = await Mediator.Send(getAllIncomeQuery);
            return Ok(getIncomes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IncomeViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<IncomeViewModel>> GetByIdAsync(string id)
        {
            var incomeVm = new IncomeViewModel
            {
                UpdateModel = await Mediator.Send(new GetIncomeDetailQuery { Id = id })
            };

            incomeVm.OptionsDataSources.PaymentMoodSelectList = await Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.PaymentMode });
            incomeVm.OptionsDataSources.MonthSelectList = await Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.Month });
            return Ok(incomeVm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IncomeCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Income", "Create")]
        public async Task<ActionResult<IncomeCreateModel>> CreateAsync(CreateIncomeCommand incomeCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createIncome = await Mediator.Send(incomeCreateModel);
                return Ok(createIncome);
            }

            return BadRequest(incomeCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(IncomeUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Income", "Update")]
        public async Task<ActionResult<IncomeUpdateModel>> UpdateAsync(UpdateIncomeCommand incomeUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateExpense = await Mediator.Send(incomeUpdateModel);
                return Ok(updateExpense);
            }

            return BadRequest(incomeUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Income", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteIncome = await Mediator.Send(new DeleteIncomeCommand { Id = id });
            return Ok(deleteIncome);
        }
    }
}