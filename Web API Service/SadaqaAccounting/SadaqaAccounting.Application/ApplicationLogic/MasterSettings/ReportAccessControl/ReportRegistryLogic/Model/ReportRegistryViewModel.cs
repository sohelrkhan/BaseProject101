namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Model
{
    public class ReportRegistryViewModel
    {
        public ReportRegistryCreateModel CreateModel { get; set; }
        public ReportRegistryUpdateModel UpdateModel { get; set; }
        public ReportRegistryGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class ReportRegistryCreateModel : IMapFrom<ReportRegistry>
    {
        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(250, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        [Required(ErrorMessage = "Please, provide url.")]
        public string Url { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide report code.")]
        public string ReportCode { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Please, provide report group.")]
        public string ReportGroup { get; set; }

        [Required(ErrorMessage = "Please, select module.")]
        public int ModuleId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ReportRegistry, ReportRegistryCreateModel>();
            profile.CreateMap<ReportRegistryCreateModel, ReportRegistry>();
        }
    }

    public class ReportRegistryUpdateModel : IMapFrom<ReportRegistry>
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(250, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        [Required(ErrorMessage = "Please, provide url.")]
        public string Url { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide report code.")]
        public string ReportCode { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Please, provide report group.")]
        public string ReportGroup { get; set; }

        [Required(ErrorMessage = "Please, select module.")]
        public int ModuleId { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ReportRegistry, ReportRegistryUpdateModel>();
            profile.CreateMap<ReportRegistryUpdateModel, ReportRegistry>();
        }
    }

    public class ReportRegistryGridModel : IMapFrom<ReportRegistry>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ReportCode { get; set; }
        public string ReportGroup { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool IsChecked { get; set; } = false;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ReportRegistry, ReportRegistryGridModel>()
                .ForMember(d => d.ModuleName, s => s.MapFrom(m => m.Module.Name))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
            profile.CreateMap<ReportRegistryGridModel, ReportRegistry>();
        }
    }

    public class GroupReportRegistryModel
    {
        public string GroupName { get; set; }
        public ICollection<ReportRegistryGridModel> ReportRegistryList { get; set; }
    }
}