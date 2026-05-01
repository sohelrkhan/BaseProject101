namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("Notifications", Schema = "MasterSettings")]
    public class Notifications : IAuditableEntity, IDelatableEntity
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
        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }
}