namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.FeaturesLogic.Model
{
    public class FeaturesViewModel
    {
        public FeaturesCreateModel CreateModel { get; set; }
        public FeaturesUpdateModel UpdateModel { get; set; }
        public FeaturesGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class FeaturesCreateModel : IMapFrom<Feature>
    {
        [Required(ErrorMessage = "Please, select module.")]
        public int ModuleId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide code.")]
        [StringLength(500, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(500, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide linked table name.")]
        [StringLength(50, MinimumLength = 2)]
        public string LinkedTableName { get; set; }

        public string? LinkedControllerName { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Feature, FeaturesCreateModel>();
            profile.CreateMap<FeaturesCreateModel, Feature>();
        }
    }

    public class FeaturesUpdateModel : IMapFrom<Feature>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }

        [Required(ErrorMessage = "Please, select module.")]
        public int ModuleId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide code.")]
        [StringLength(500, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(500, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide linked table name.")]
        [StringLength(50, MinimumLength = 2)]
        public string LinkedTableName { get; set; }

        public string? LinkedControllerName { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Feature, FeaturesUpdateModel>();
            profile.CreateMap<FeaturesUpdateModel, Feature>();
        }
    }

    public class FeaturesGridModel : IMapFrom<Feature>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }
        public string ModuleName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string LinkedTableName { get; set; }
        public string? LinkedControllerName { get; set; }
        public string StatusName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Feature, FeaturesGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.ModuleName, s => s.MapFrom(m => m.Module.Name))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
        }
    }
}