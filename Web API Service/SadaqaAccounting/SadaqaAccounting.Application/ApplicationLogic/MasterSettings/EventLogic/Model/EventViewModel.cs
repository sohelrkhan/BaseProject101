namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Model
{
    public class EventViewModel
    {
        public EventCreateModel CreateModel { get; set; }
        public EventUpdateModel UpdateModel { get; set; }
        public EventGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }
    public class EventCreateModel: IMapFrom<Event>
    {
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        public string? StartDateString { get; set; }
        public string? EndDateString { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Event, EventCreateModel>();
            profile.CreateMap<EventCreateModel, Event>();
        }
    }
    public class EventUpdateModel: IMapFrom<Event>
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        public string? StartDateString { get; set; }
        public string? EndDateString { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Event, EventUpdateModel>();
            profile.CreateMap<EventUpdateModel, Event>();
        }
    }
    public class EventGridModel : IMapFrom<Event>
    {
        public int Id { get; set; }
        public int AccountUnitId { get; set; }
        public string EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        public string? StartDateString { get; set; }
        public string? EndDateString { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Event, EventGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())));
            profile.CreateMap<EventGridModel, Event>();
        }
    }

}
