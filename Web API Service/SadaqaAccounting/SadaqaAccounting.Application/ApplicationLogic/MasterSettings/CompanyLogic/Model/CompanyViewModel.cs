namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Model
{
    public class CompanyViewModel
    {
        public CompanyCreateModel CreateModel { get; set; }
        public CompanyUpdateModel UpdateModel { get;set; }
        public CompanyGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class CompanylogoUpdateModel
    {
        public int Id { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class CompanyCreateModel : IMapFrom<Company>
    {
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public string? Logo { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide country.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Country { get; set; }

        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Company, CompanyCreateModel>();
            profile.CreateMap<CompanyCreateModel, Company>();
        }
    }

    public class CompanyUpdateModel : IMapFrom<Company>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public string? Logo { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide country.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Country { get; set; }

        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Company, CompanyUpdateModel>();
            profile.CreateMap<CompanyUpdateModel, Company>();
        }
    }

    public class CompanyGridModel : IMapFrom<Company>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string Country { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Company, CompanyGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
        }
    }
}