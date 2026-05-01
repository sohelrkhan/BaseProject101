namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.NotificationLogic.Model
{
    public class NotificationViewModel
    {
        public NotificationCreateModel CreateModel { get; set; }
        public NotificationUpdateModel UpdateModel { get; set; }
        public NotificationGridModel GridModel { get; set; }
    }

    public class NotificationCreateModel : IMapFrom<Notifications>
    {
        [Required(ErrorMessage = "Please, provide title.")]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; }

        public string? Message { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? SenderUserId { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Notifications, NotificationCreateModel>();
            profile.CreateMap<NotificationCreateModel, Notifications>();   
        }
    }

    public class NotificationUpdateModel : IMapFrom<Notifications>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, provide title.")]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; }

        public string? Message { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? SenderUserId { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Notifications, NotificationUpdateModel>();
            profile.CreateMap<NotificationUpdateModel, Notifications>();
        }
    }

    public class NotificationGridModel : IMapFrom<Notifications>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Message { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? SenderUserId { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Notifications, NotificationGridModel>();
            profile.CreateMap<NotificationGridModel, Notifications>();
        }
    }
}