namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class EmployeeController : BaseController
    {
        [HttpPost]
        [CheckAuthorize("Employee", "List")]
        public async Task<ActionResult<PaginatedResponse<EmployeeGridModel>>> GetPaginatedAsync(GetPaginatedEmployeesQuery request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeeViewModel>> GetByIdAsync(string id)
        {
            var employeeViewModel = new EmployeeViewModel
            {
                UpdateModel = await Mediator.Send(new GetEmployeeDetailsQuery { Id = id })
            };

            // Select list         
            employeeViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;
            employeeViewModel.OptionsDataSources.CompanySelectList = await Mediator.Send(new SelectListCompanyQuery());

            return Ok(employeeViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [CheckAuthorize("Employee", "Create")]
        public async Task<ActionResult<int>> CreateAsync(CreateEmployeeCommand employeeCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createEmployeeId = await Mediator.Send(employeeCreateModel);
                return Ok(createEmployeeId);
            }

            return BadRequest(employeeCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(EmployeeUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Employee", "Update")]
        public async Task<ActionResult<EmployeeUpdateModel>> UpdateAsync(UpdateEmployeeCommand employeeUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateEmployee = await Mediator.Send(employeeUpdateModel);
                return Ok(updateEmployee);
            }

            return BadRequest(employeeUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Employee", "Delete")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            var deleteEmployee = await Mediator.Send(new DeleteEmployeeCommand { Id = id });
            return Ok(deleteEmployee);
        }

        [HttpPut]
        [ProducesResponseType(typeof(EmployeeImageUpdateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeeImageUpdateModel>> EmployeeProfileImageUpdateAsync([FromForm] UpdateEmployeeImageCommand employeeImageUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateEmployeeImage = await Mediator.Send(employeeImageUpdateModel);
                return Ok(updateEmployeeImage);
            }

            return BadRequest(employeeImageUpdateModel);
        }
    }
}