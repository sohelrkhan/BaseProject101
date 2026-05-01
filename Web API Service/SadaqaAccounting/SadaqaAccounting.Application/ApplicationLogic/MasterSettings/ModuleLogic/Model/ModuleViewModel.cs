namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Model
{
    public class ModuleViewModel
    {
        public ModuleCreateModel CreateModel { get; set; }  
        public ModuleUpdateModel UpdateModel { get; set; }
        public ModuleGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class ModuleCreateModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.Module>
    {
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide unique code.")]
        [StringLength(50, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.Module, ModuleCreateModel>();
            profile.CreateMap<ModuleCreateModel, SadaqaAccounting.Model.Models.MasterSettings.Module>();
        }
    }

    public class ModuleUpdateModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.Module>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide unique code.")]
        [StringLength(50, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.Module, ModuleUpdateModel>()
                .ForMember(d => d.EncryptedId,s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())));
            profile.CreateMap<ModuleUpdateModel, SadaqaAccounting.Model.Models.MasterSettings.Module>();
        }
    }

    public class ModuleGridModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.Module>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.Module, ModuleGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
            profile.CreateMap<ModuleGridModel, SadaqaAccounting.Model.Models.MasterSettings.Module>();
        }
    }
}