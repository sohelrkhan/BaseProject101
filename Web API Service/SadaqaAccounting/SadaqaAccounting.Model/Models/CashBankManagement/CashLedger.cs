namespace SadaqaAccounting.Model.Models.Cash_BankManagement
{
    [Table("CashLedgers", Schema = "Cash&BankManagement")]
    public class CashLedger: IAuditableEntity, IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, provide transaction date.")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Please, provide transaction type.")]
        public int TransactionTypeId { get; set; }

        [Required(ErrorMessage = "Please, provide source type.")]
        public int SourceTypeId { get; set; }

        [Required(ErrorMessage = "Please, provide source.")]
        public int SourceId { get; set; }

        [Required(ErrorMessage = "Please, provide cash.")]
        public int CashId { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public EnumTypeCollection TransactionType { get; set; }
        public EnumTypeCollection SourceType { get; set; }
        public Cash Cash { get; set; }
    }
}