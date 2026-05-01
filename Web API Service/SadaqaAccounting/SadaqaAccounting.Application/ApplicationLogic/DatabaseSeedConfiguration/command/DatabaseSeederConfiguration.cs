namespace SadaqaAccounting.Application.ApplicationLogic.DatabaseSeedConfiguration.command
{
    public class DatabaseSeederConfiguration
    {
        private readonly CreateEnumSeedCommand _createEnumSeedCommand;
        private readonly CreateActionSeedCommand _createActionSeedCommand;
        private readonly CreateFeatureSeedCommand _createFeatureSeedCommand;
        private readonly CreateFeatureActionMappingSeedCommand _createFeatureActionMappingSeedCommand;
        private readonly CreateCompanySeedCommand _createCompanySeedCommand;
        private readonly CreateSuperAdminUserCommand _createSuperAdminUserCommand;              
        private readonly CreateUserAccessMappingSeedCommand _createUserAccessMappingSeedCommand;    
        private readonly CreateAccountUnitSeedCommand _createAccountUnitSeedCommand;
        private readonly CreateReportRegistrySeedCommand _createReportRegistrySeedCommand;

        public DatabaseSeederConfiguration(
            CreateEnumSeedCommand createEnumSeedCommand, 
            CreateActionSeedCommand createActionSeedCommand, 
            CreateFeatureSeedCommand createFeatureSeedCommand, 
            CreateFeatureActionMappingSeedCommand createFeatureActionMappingSeedCommand, 
            CreateCompanySeedCommand createCompanySeedCommand, 
            CreateSuperAdminUserCommand createSuperAdminUserCommand, 
            CreateUserAccessMappingSeedCommand createUserAccessMappingSeedCommand, 
            CreateAccountUnitSeedCommand createAccountUnitSeedCommand, 
            CreateReportRegistrySeedCommand createReportRegistrySeedCommand)
        {
            _createEnumSeedCommand = createEnumSeedCommand;
            _createActionSeedCommand = createActionSeedCommand;
            _createFeatureSeedCommand = createFeatureSeedCommand;
            _createFeatureActionMappingSeedCommand = createFeatureActionMappingSeedCommand;
            _createCompanySeedCommand = createCompanySeedCommand;
            _createSuperAdminUserCommand = createSuperAdminUserCommand;
            _createUserAccessMappingSeedCommand = createUserAccessMappingSeedCommand;
            _createAccountUnitSeedCommand = createAccountUnitSeedCommand;
            _createReportRegistrySeedCommand = createReportRegistrySeedCommand;
        }

        public async Task SeedAsync()
        {
            // Seed each domain in order (if necessary)
            await _createEnumSeedCommand.SeedAsync();
            await _createActionSeedCommand.SeedAsync();
            await _createFeatureSeedCommand.SeedAsync();
            await _createFeatureActionMappingSeedCommand.SeedAsync();
            await _createCompanySeedCommand.SeedAsync();
            await _createSuperAdminUserCommand.SeedAsync();
            await _createUserAccessMappingSeedCommand.SeedAsync();
            await _createAccountUnitSeedCommand.SeedAsync();
            await _createReportRegistrySeedCommand.SeedAsync();
        }
    }
}