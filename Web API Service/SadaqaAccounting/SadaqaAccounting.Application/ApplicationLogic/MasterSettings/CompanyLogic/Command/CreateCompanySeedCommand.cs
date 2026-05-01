namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Command
{
    public class CreateCompanySeedCommand
    {
        private readonly ICompanyRepository _companyRepository;

        public CreateCompanySeedCommand(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task SeedAsync()
        {
            var company = new Company
            {
                Name = "10 No Central Mosque",
                Logo = string.Empty,
                Country = "Bangladesh",
                Website = string.Empty,
                Email = string.Empty,
                Mobile = string.Empty,
                Address = string.Empty,
                StatusId = GlobalStatus.Active,
                CreatedById = "Super Admin",
                CreatedDateTime = DateTime.UtcNow
            };

            // Check if company already exists
            var getAllCompany = await _companyRepository.GetAllAsync();
            var isExist = getAllCompany.FirstOrDefault(x => x.Name.Equals(company.Name));

            if (isExist == null)
                await _companyRepository.CreateAsync(company);
        }
    }
}