namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("AccountUnits", Schema = "MasterSettings")]
    public class AccountUnit: IAuditableEntity, IDelatableEntity
    {
        public AccountUnit()
        {
            UserAccountUnits = new HashSet<UserAccountUnit>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public ICollection<UserAccountUnit> UserAccountUnits { get; set; }
    }
}