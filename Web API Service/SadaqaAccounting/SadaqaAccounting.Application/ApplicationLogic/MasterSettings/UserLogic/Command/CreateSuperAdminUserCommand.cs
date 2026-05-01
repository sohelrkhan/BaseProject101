namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Command
{
    public class CreateSuperAdminUserCommand
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateSuperAdminUserCommand(UserManager<User> userManager, IEmployeeRepository employeeRepository)
        {
            _userManager = userManager;
            _employeeRepository = employeeRepository;
        }

        public async Task SeedAsync()
        {
            // Fix id for supper admin
            var id = "a6923057-af80-4dd0-b3b2-3ff979f69b6d";

            // Fix email for supper admin
            var isEmailExist = await _employeeRepository.IsEmailExistAsync("super_admin@gmail.com");

            // Create Super Admin Employee
            if (!isEmailExist)
            {
                var superAdminEmployee = new SadaqaAccounting.Model.Models.MasterSettings.Employee
                {
                    FullName = "Super Admin",
                    CompanyId = 1,
                    Email = "super_admin@gmail.com",
                    PhoneNumber = string.Empty,
                    Image = string.Empty,
                    Address = string.Empty,
                    StatusId = GlobalStatus.Active,
                    CreatedById = "System",
                    CreatedDateTime = DateTime.UtcNow,
                };

                // Save Employee
                var createdEmployee = await _employeeRepository.CreateAsync(superAdminEmployee);

                var registerUser = new User();
                registerUser.Id = id;
                registerUser.UserName = "super_admin@gmail.com";
                registerUser.Email = "super_admin@gmail.com";
                registerUser.FullName = "Super Admin";
                registerUser.EmployeeId = createdEmployee.Id;
                registerUser.LockoutEnabled = false;
                registerUser.ForcePasswordChanged = true;
                registerUser.ApplicationUserTypeId = ApplicationUserType.Admin;

                // Save user
                await _userManager.CreateAsync(registerUser, "9qDH121P7%yF7@*");
            }
        }
    }
}