namespace SadaqaAccounting.Api.Controllers.ExpenseManagement
{
    public class ExpenseController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<ExpenseGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Expense", "List")]
        public async Task<ActionResult<PaginatedResponse<ExpenseGridModel>>> GetAllAsync(GetAllExpenseQuery getAllExpenseQuery)
        {
            var getExpense = await Mediator.Send(getAllExpenseQuery);
            return Ok(getExpense);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExpenseViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExpenseViewModel>> GetByIdAsync(string id)
        {
            var expenseVm = new ExpenseViewModel
            {
                UpdateModel = await Mediator.Send(new GetExpenseDetailQuery { Id = id })
            };

            expenseVm.OptionsDataSources.PaymentMoodSelectList = await Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.PaymentMode });
            return Ok(expenseVm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ExpenseCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Expense", "Create")]
        public async Task<ActionResult<ExpenseCreateModel>> CreateAsync(CreateExpenseCommand expenseCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createExpense = await Mediator.Send(expenseCreateModel);
                return Ok(createExpense);
            }

            return BadRequest(expenseCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ExpenseUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Expense", "Update")]
        public async Task<ActionResult<ExpenseUpdateModel>> UpdateAsync(UpdateExpenseCommand expenseUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateExpense = await Mediator.Send(expenseUpdateModel);
                return Ok(updateExpense);
            }

            return BadRequest(expenseUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Expense", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteExpense = await Mediator.Send(new DeleteExpenseCommand { Id = id });
            return Ok(deleteExpense);
        }
    }
}