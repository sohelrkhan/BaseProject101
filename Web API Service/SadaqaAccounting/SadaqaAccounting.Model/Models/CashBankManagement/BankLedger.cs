namespace SadaqaAccounting.Model.Models.Cash_BankManagement
{
    [Table("BankLedgers", Schema = "Cash&BankManagement")]
    public class BankLedger : IAuditableEntity, IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, select a bank.")]
        public int BankId { get; set; }

        public DateTime TransactionDate { get; set; }
        public int TransactionTypeId { get; set; }
        public int SourceTypeId { get; set; }
        public int SourceId { get; set; }
        public decimal Amount { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public Bank Bank { get; set; }
        public EnumTypeCollection TransactionType { get; set; }
        public EnumTypeCollection SourceType { get; set; }
    }
}