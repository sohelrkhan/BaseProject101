namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Model
{
    public class ActionViewModel
    {
        public ActionCreateModel CreateModel { get; set; }
        public ActionUpdateModel UpdateModel { get; set; }
        public ActionGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class ActionCreateModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>
    {
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action, ActionCreateModel> ();
            profile.CreateMap<ActionCreateModel, SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action> ();
        }        
    }

    public class ActionUpdateModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, provide status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action, ActionUpdateModel>();
            profile.CreateMap<ActionUpdateModel, SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>();
        }
    }

    public class ActionGridModel : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }
        public string Name { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool IsExist { get; set; }
        public bool IsChecked { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action, ActionGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
            profile.CreateMap<ActionGridModel, SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>();
        }
    }
}