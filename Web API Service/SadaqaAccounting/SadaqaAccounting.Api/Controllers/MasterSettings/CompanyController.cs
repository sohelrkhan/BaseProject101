namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class CompanyController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<CompanyGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Company", "List")]
        public async Task<ActionResult<ICollection<CompanyGridModel>>> GetAllAsync()
        {
            var getCompanies = await Mediator.Send(new GetAllCompanyQuery());
            return Ok(getCompanies);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanyViewModel>> GetByIdAsync(string id)
        {
            var companyViewModel = new CompanyViewModel
            {
                UpdateModel = await Mediator.Send(new GetCompanyDetailsQuery { Id = id })
            };

            // Select list
            companyViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;
            return Ok(companyViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CompanyCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Company", "Create")]
        public async Task<ActionResult<CompanyCreateModel>> CreateAsync(CreateCompanyCommand companyCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createCompany = await Mediator.Send(companyCreateModel);
                return Ok(createCompany);
            }

            return BadRequest(companyCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CompanyUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Company", "Update")]
        public async Task<ActionResult<CompanyUpdateModel>> UpdateAsync(UpdateCompanyCommand companyUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateCompany = await Mediator.Send(companyUpdateModel);
                return Ok(updateCompany);
            }

            return BadRequest(companyUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Company", "Delete")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            var deleteCompany = await Mediator.Send(new DeleteCompanyCommand { Id = id });
            return Ok(deleteCompany);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CompanylogoUpdateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanylogoUpdateModel>> CompanyLogoUpdateAsync([FromForm] CompanylogoUpdateCommand logoUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var logoUpdateModelImage = await Mediator.Send(logoUpdateModel);
                return Ok(logoUpdateModelImage);
            }
            return BadRequest(logoUpdateModel);
        }
    }
}