namespace SadaqaAccounting.Model.Models.MasterSettings.ReportAccessControl
{
    [Table("ReportUserAccess", Schema = "MasterSettings")]
    public class ReportUserAccess : IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select report.")]
        public int ReportRegistryId { get; set; }

        [Required(ErrorMessage = "Please, select user.")]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public ReportRegistry ReportRegistry { get; set; }
        public User User { get; set; }
    }
}