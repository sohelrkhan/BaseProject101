namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Command
{
    public class CreateReportRegistrySeedCommand
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IMapper _mapper;
        private readonly IReportRegistryRepository _reportRegistryRepository;
        public CreateReportRegistrySeedCommand(IModuleRepository moduleRepository, IMapper mapper, IReportRegistryRepository reportRegistryRepository)
        {
            _moduleRepository = moduleRepository;
            _mapper = mapper;
            _reportRegistryRepository = reportRegistryRepository;
        }

        public async Task SeedAsync()
        {
            // Get newly Report Registry Collection
            var reportRegistries = await CreateReportRegistry();

            // For Report Registry Table
            foreach (var reportRegistry in reportRegistries)
            {
                // Check report is exist or not
                var isReportExit = await _reportRegistryRepository.GetByReportCodeAsync(reportRegistry.ReportCode);

                if (isReportExit == null)
                {
                    var newReport = _mapper.Map<ReportRegistry>(reportRegistry);
                    await _reportRegistryRepository.CreateAsync(newReport);
                }
            }
        }

        // Create seed Report Registry
        private async Task<ICollection<ReportRegistry>> CreateReportRegistry()
        {
            var getAllModules = await _moduleRepository.GetAllAsync();

            var reportModule = getAllModules.FirstOrDefault(m => m.Code == "RPT");

            return new List<ReportRegistry>()
            {
                #region Report Module
                //Income Expense Report
                new ReportRegistry { ModuleId = reportModule!.Id, ReportCode = "MIER", ReportGroup = "Reports", Name = "Income & Expense",
                    Url = "/app/income-expense-report", StatusId = GlobalStatus.Active},
                #endregion
            };
        }
    }
}
