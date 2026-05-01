namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportUserAccessLogic.Model
{
    public class ReportUserAccessViewModel
    {
        public ReportUserAccessCreateModel CreateModel { get; set; }
        public ReportRegistryUserAccessGridModel GridModel { get; set; }
    }

    public class ReportUserAccessCreateModel : IMapFrom<ReportUserAccess>
    {
        public int[] ReportIds { get; set; }

        [Required(ErrorMessage = "Please, select user.")]
        public string UserId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ReportUserAccess, ReportUserAccessCreateModel>();
            profile.CreateMap<ReportUserAccessCreateModel, ReportUserAccess>();
        }
    }

    public class ReportRegistryUserAccessGridModel
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ModuleName { get; set; }
        public string ReportCode { get; set; }
        public string ReportGroup { get; set; }
        public string UserId { get; set; }
        public bool IsChecked { get; set; } = false;
    }

    public class ReportUserAccessGridModel : IMapFrom<ReportUserAccess>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select report.")]
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ModuleName { get; set; }
        public string ReportCode { get; set; }
        public string ReportGroup { get; set; }

        [Required(ErrorMessage = "Please, select user.")]
        public string UserId { get; set; }
        public string UserName { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ReportUserAccess, ReportUserAccessGridModel>()
                .ForMember(d => d.ReportName, s => s.MapFrom(m => m.ReportRegistry.Name))
                .ForMember(d => d.ModuleName, s => s.MapFrom(m => m.ReportRegistry.Module.Name))
                .ForMember(d => d.ReportCode, s => s.MapFrom(m => m.ReportRegistry.ReportCode))
                .ForMember(d => d.ReportGroup, s => s.MapFrom(m => m.ReportRegistry.ReportGroup))
                .ForMember(d => d.UserName, s => s.MapFrom(m => m.User.FullName));
            profile.CreateMap<ReportUserAccessGridModel, ReportUserAccess>();
        }
    }
}