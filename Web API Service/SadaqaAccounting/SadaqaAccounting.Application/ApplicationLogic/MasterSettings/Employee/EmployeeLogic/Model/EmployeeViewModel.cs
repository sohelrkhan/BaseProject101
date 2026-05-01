namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Model
{
    public class EmployeeViewModel
    {
        public EmployeeCreateModel CreateModel { get; set; }    
        public EmployeeUpdateModel UpdateModel { get; set; }    
        public EmployeeGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class EmployeeCreateModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.Employee>
    {
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(50, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please, provide company.")]
        public int CompanyId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.Employee, EmployeeCreateModel>();
            profile.CreateMap<EmployeeCreateModel, SadaqaAccounting.Model.Models.MasterSettings.Employee>();
        }
    }

    public class EmployeeUpdateModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.Employee>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(50, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please, provide company.")]
        public int CompanyId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.Employee, EmployeeUpdateModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())));
            profile.CreateMap<EmployeeUpdateModel, SadaqaAccounting.Model.Models.MasterSettings.Employee>();
        }
    }

    public class EmployeeGridModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.Employee>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool HasUserId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.Employee, EmployeeGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.CompanyName, s => s.MapFrom(m => m.Company.Name))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
        }
    }

    public class EmployeeImageUpdateModel
    {
        public int Id { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}