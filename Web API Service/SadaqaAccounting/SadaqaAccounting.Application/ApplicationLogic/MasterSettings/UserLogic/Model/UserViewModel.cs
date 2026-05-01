namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Model
{
    public class UserViewModel
    {
        public UserRegistrationModel UserRegistrationModel { get; set; }
        public UserCreateModel UserCreateModel { get; set; }
        public UserUpdateModel UserUpdateModel { get; set; }
        public UserGridModel UserGridModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class UserRegistrationModel
    {
        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, provide password.")]
        [DataType(DataType.Password, ErrorMessage = "Please, provide valid password.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string FullName { get; set; }
        public int? EmployeeId { get; set; }
        public bool ForcePasswordChanged { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, provide password.")]
        [DataType(DataType.Password, ErrorMessage = "Please, provide valid password.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }
    }

    public class UserCreateModel : IMapFrom<User>
    {
        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, provide password.")]
        [DataType(DataType.Password, ErrorMessage = "Please, provide valid password.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string FullName { get; set; }
        public int? EmployeeId { get; set; }
        public int? BuyerId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserCreateModel>();
            profile.CreateMap<UserCreateModel, User>();
        }
    }

    public class UserUpdateModel : IMapFrom<User>
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string FullName { get; set; }
        public int? EmployeeId { get; set; }
        public bool ForcePasswordChanged { get; set; }
        public bool LockoutEnabled { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserUpdateModel>();
            profile.CreateMap<UserUpdateModel, User>();
        }
    }

    public class UserGridModel : IMapFrom<User>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool LockOutEnabled { get; set; }

        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int? UniqueEmployeeId { get; set; }
        public string? EmployeeCodePrefix { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserGridModel>();
            profile.CreateMap<UserGridModel, User>();
        }
    }

    public class UserCreateFromEmployeeModel
    {
        public int EmployeeId { get; set; }
    }
}