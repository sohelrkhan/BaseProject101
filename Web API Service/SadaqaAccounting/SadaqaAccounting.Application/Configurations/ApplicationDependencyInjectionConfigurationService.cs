using SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Command;
﻿using SadaqaAccounting.Repository.Contracts.AssetManagement;
using SadaqaAccounting.Repository.Repository.AssetManagement;

namespace SadaqaAccounting.Application.Configurations
{
    public static class ApplicationDependencyInjectionConfigurationService
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // Add auto mapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutomapperMappingProfile>();
            }, Assembly.GetExecutingAssembly());

            // Add mediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            // Register IHttpContextAccessor 
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register Image Service 
            services.AddSingleton<IImageService, ImageService>();

            #region Master Setting
            services.AddScoped<IEnumTypeRepository, EnumTypeRepository>();
            services.AddScoped<IEnumTypeCollectionRepository, EnumTypeCollectionRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IActionRepository, ActionRepository>();
            services.AddScoped<IFeaturesRepository, FeaturesRepository>();
            services.AddScoped<IFeatureActionMappingRepository, FeatureActionMappingRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleActionMappingRepository, RoleActionMappingRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();        
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();         
            services.AddScoped<IUserAccessMappingRepository, UserAccessMappingRepository>();                               
            services.AddScoped<INotificationRepository, NotificationRepository>();           
            services.AddScoped<IReportRegistryRepository, ReportRegistryRepository>();
            services.AddScoped<IReportUserAccessRepository, ReportUserAccessRepository>();
            services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();

            // Register other repositories as needed
            services.AddScoped<IAccountUnitRepository, AccountUnitRepository>();
            services.AddScoped<IUserAccountUnitRepository, UserAccountUnitRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            #endregion

            #region Crypto Service
            services.AddSingleton<CryptoService>();
            #endregion

            #region Register Seed
            services.AddScoped<DatabaseSeederConfiguration>();
            services.AddScoped<CreateEnumSeedCommand>();
            services.AddScoped<CreateActionSeedCommand>();
            services.AddScoped<CreateFeatureSeedCommand>();
            services.AddScoped<CreateFeatureActionMappingSeedCommand>();
            services.AddScoped<CreateCompanySeedCommand>();
            services.AddScoped<CreateSuperAdminUserCommand>();
            services.AddScoped<CreateUserAccessMappingSeedCommand>();
            services.AddScoped<CreateAccountUnitSeedCommand>();
            services.AddScoped<CreateReportRegistrySeedCommand>();
            #endregion

            #region Expense Management
            services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            #endregion

            #region Income Management
            services.AddScoped<IIncomeCategoryRepository, IncomeCategoryRepository>();
            services.AddScoped<IIncomeRepository, IncomeRepository>();
            #endregion

            #region Donor Management
            services.AddScoped<IDonorRepository, DonorRepository>();
            #endregion

            #region CashBankManagement
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<ICashLedgerRepository, CashLedgerRepository>();
            services.AddScoped<IBankLedgerRepository, BankLedgerRepository>();
            services.AddScoped<ICashRepository, CashRepository>();
            services.AddScoped<IOpeningBalanceRepository, OpeningBalanceRepository>();
            #endregion

            #region Asset Management
            services.AddScoped<IAssettRepository, AssettRepository>();
            #endregion

            // Notification Service - AddSignalR
            services.AddSignalR();

            return services;
        }
    }
}