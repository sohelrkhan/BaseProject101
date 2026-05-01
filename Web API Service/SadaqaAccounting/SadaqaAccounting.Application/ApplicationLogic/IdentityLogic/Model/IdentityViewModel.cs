namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.Model
{
    public class IdentityViewModel
    {
        public UserModel UserModel { get; set; }
        public RegisterModel RegisterModel { get; set; }
        public LoginModel LoginModel { get; set; }
    }

    public class RegisterModel : IMapFrom<User>
    {
        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [RegularExpression("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please, provied valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, provied user name.")]
        [StringLength(20, MinimumLength = 2)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please, provide password.")]
        [DataType(DataType.Password, ErrorMessage = "Please, provide valid password.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please, provied valid password.")]
        [Compare("Password", ErrorMessage = "Password cannot matched! Try again.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string FullName { get; set; }
        public int? EmployeeId { get; set; }
        public bool ForcePasswordChanged { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, RegisterModel>();
            profile.CreateMap<RegisterModel, User>();
        }
    }

    public class ChangePassword : IMapFrom<User>
    {
        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [RegularExpression("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please, provied valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please, provide password.")]
        [DataType(DataType.Password, ErrorMessage = "Please, provide valid password.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please, provied valid password.")]
        [Compare("Password", ErrorMessage = "Password cannot matched! Try again.")]
        public string ConfirmPassword { get; set; }

        public bool ForcePasswordChanged { get; set; }
    }

    public class LoginModel : IMapFrom<User>
    {
        [Required(ErrorMessage = "Please, provide email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please, provide valid email address.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, provide password.")]
        [DataType(DataType.Password, ErrorMessage = "Please, provide valid password.")]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, LoginModel>();
            profile.CreateMap<LoginModel, User>();
        }
    }

    public class UserModel : IMapFrom<User>
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeEncryptedId { get; set; }
        public int? CompanyId { get; set; }
        public bool ForcePasswordChanged { get; set; }
        public int? ApplicationUserTypeId { get; set; }
        public string? Image { get; set; }
        public int? AccountUnitId { get; set; }

        public ICollection<UserAccessMappingDetails> UserAccessMappingDetails { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserModel>();
        }
    }   
}