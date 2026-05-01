namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Command
{
    public class CreateFeatureSeedCommand
    {
        private readonly IFeaturesRepository _featureRepository;
        private readonly IModuleRepository _moduleRepository;

        public CreateFeatureSeedCommand(IFeaturesRepository featureRepositopry, IModuleRepository moduleRepository)
        {
            _featureRepository = featureRepositopry;
            _moduleRepository = moduleRepository;
        }

        public async Task SeedAsync()
        {
            var featureList = new List<Feature>();

            // Get newly features and modules
            var createdModules = CreateModule();
            var masterSettings = MasterSettings();

            // For module
            foreach (var module in createdModules)
            {
                // Check module is exist or not
                var isModuleExit = await _moduleRepository.GetModuleByName(module.Name);
                int moduleId = 0;

                if (isModuleExit == null)
                {
                    var result = await _moduleRepository.CreateAsync(module);
                    moduleId = result.Id;
                }
                else
                    moduleId = isModuleExit.Id;

                switch (module.Code)
                {
                    case "MS":
                        {
                            // For feature
                            foreach (var masterSetting in masterSettings)
                            {
                                // Check feature is exist or not
                                var isFeatureExit = await _featureRepository.GetFeatureByTableName(masterSetting.LinkedTableName);

                                if (isFeatureExit == null || string.Equals(isFeatureExit.LinkedTableName, "Default"))
                                {
                                    masterSetting.ModuleId = moduleId;
                                    featureList.Add(masterSetting);
                                }
                            }
                        }
                        break;
                }
            }
            // bulk insert features
            await _featureRepository.CreateBulkInsertAsync(featureList);
        }

        // Create seed module
        private ICollection<SadaqaAccounting.Model.Models.MasterSettings.Module> CreateModule()
        {
            return new List<SadaqaAccounting.Model.Models.MasterSettings.Module>()
            {
                new SadaqaAccounting.Model.Models.MasterSettings.Module { Code = "MS", Name = "Master Setting", 
                    StatusId = GlobalStatus.Active },
                new SadaqaAccounting.Model.Models.MasterSettings.Module { Code = "EM", Name = "Expense Management", 
                    StatusId = GlobalStatus.Active },
                new SadaqaAccounting.Model.Models.MasterSettings.Module { Code = "IM", Name = "Income Management", 
                    StatusId = GlobalStatus.Active },
                new SadaqaAccounting.Model.Models.MasterSettings.Module { Code = "CBM", Name = "Cash Bank Management", 
                    StatusId = GlobalStatus.Active },
                new SadaqaAccounting.Model.Models.MasterSettings.Module { Code = "RPT", Name = "Reports & Analytics", 
                    StatusId = GlobalStatus.Active },
            };
        }

        private ICollection<Feature> MasterSettings()
        {
            return new List<Feature>()
            {
                #region Master Setting Module                
                new Feature { Code = "EnumType", Name = "Enum Type", ModuleId = 1, LinkedTableName = "EnumTypes", LinkedControllerName = "EnumType", StatusId = GlobalStatus.Active },
                new Feature { Code = "EnumTypeCollection", Name = "Enum Type Collection", ModuleId = 1, LinkedTableName = "EnumTypeCollections", LinkedControllerName = "EnumTypeCollection", StatusId = GlobalStatus.Active },
                new Feature { Code = "Action", Name = "Action", ModuleId = 1, LinkedTableName = "Actions", LinkedControllerName = "Action", StatusId = GlobalStatus.Active },
                new Feature { Code = "Role", Name = "Role", ModuleId = 1, LinkedTableName = "Roles",LinkedControllerName = "Role", StatusId = GlobalStatus.Active },
                new Feature { Code = "Feature", Name = "Feature", ModuleId = 1, LinkedTableName = "Features", LinkedControllerName = "Feature", StatusId = GlobalStatus.Active },
                new Feature { Code = "FeatureActionMapping", Name = "Feature Action Mapping", ModuleId = 1, LinkedTableName = "FeatureActionMappings", LinkedControllerName = "FeatureActionMapping", StatusId = GlobalStatus.Active },
                new Feature { Code = "RoleActionMapping", Name = "Role Action Mapping", ModuleId = 1, LinkedTableName = "RoleActionMappings", LinkedControllerName = "RoleActionMapping", StatusId = GlobalStatus.Active },
                new Feature { Code = "UserAccessMapping", Name = "User Access Mapping", ModuleId = 1, LinkedTableName = "UserAccessMappings", LinkedControllerName = "UserAccessMapping", StatusId = GlobalStatus.Active },
                new Feature { Code = "UserAccountUnit", Name = "User Account Unit", ModuleId = 1, LinkedTableName = "UserAccountUnits", LinkedControllerName = "UserAccountUnit", StatusId = GlobalStatus.Active },
                new Feature { Code = "AccountUnit", Name = "Account Unit", ModuleId = 1, LinkedTableName = "AccountUnits", LinkedControllerName = "AccountUnit", StatusId = GlobalStatus.Active },
                new Feature { Code = "Company", Name = "Company", ModuleId = 1, LinkedTableName = "Companies", LinkedControllerName = "Company", StatusId = GlobalStatus.Active },
                new Feature { Code = "Employee", Name = "Employee", ModuleId = 1, LinkedTableName = "Employees", LinkedControllerName = "Employee", StatusId = GlobalStatus.Active },
                new Feature { Code = "Donor", Name = "Donor", ModuleId = 1, LinkedTableName = "Donors", LinkedControllerName = "Donor", StatusId = GlobalStatus.Active },
                new Feature { Code = "Module", Name = "Module", ModuleId = 1, LinkedTableName = "Modules", LinkedControllerName = "Module", StatusId = GlobalStatus.Active },
                new Feature { Code = "Event", Name = "Event", ModuleId = 1, LinkedTableName = "Events", LinkedControllerName = "Event", StatusId = GlobalStatus.Active },
                new Feature { Code = "ReportRegistry", Name = "Report Registry", ModuleId = 1, LinkedTableName = "ReportRegistries", LinkedControllerName = "ReportRegistry", StatusId = GlobalStatus.Active },
                new Feature { Code = "ReportUserAccess", Name = "Report User Access", ModuleId = 1, LinkedTableName = "ReportUserAccess", LinkedControllerName = "ReportUserAccess", StatusId = GlobalStatus.Active },
                #endregion

                #region Expense Management
                new Feature { Code = "ExpenseCategory", Name = "Expense Category", ModuleId = 2, LinkedTableName = "ExpenseCategories", LinkedControllerName = "ExpenseCategory", StatusId = GlobalStatus.Active },
                new Feature { Code = "Expense", Name = "Expense", ModuleId = 2, LinkedTableName = "Expenses", LinkedControllerName = "Expense", StatusId = GlobalStatus.Active },
                #endregion

                #region Income Management
                new Feature { Code = "IncomeCategory", Name = "Income Category", ModuleId = 3, LinkedTableName = "IncomeCategories", LinkedControllerName = "IncomeCategory", StatusId = GlobalStatus.Active },
                new Feature { Code = "Income", Name = "Income", ModuleId = 3, LinkedTableName = "Incomes", LinkedControllerName = "Income", StatusId = GlobalStatus.Active },
                #endregion

                #region Cash Bank Management
                new Feature { Code = "Bank", Name = "Bank", ModuleId = 5, LinkedTableName = "Banks", 
                    LinkedControllerName = "Bank", StatusId = GlobalStatus.Active },
                 new Feature { Code = "OpeningBalance", Name = "Opening Balance", ModuleId = 5, 
                     LinkedTableName = "OpeningBalances", LinkedControllerName = "OpeningBalance", 
                     StatusId = GlobalStatus.Active }
                #endregion
            };
        }
    }
}