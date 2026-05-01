namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("UserAccountUnits",Schema = "MasterSettings")]
    public class UserAccountUnit : IDelatableEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int AccountUnitId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public User User { get; set; }
        public AccountUnit AccountUnit { get; set; }
    }
}
