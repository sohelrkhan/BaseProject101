namespace SadaqaAccounting.Model.Models.Cash_BankManagement
{
    [Table("OpeningBalances", Schema = "Cash&BankManagement")]
    public class OpeningBalance : IAuditableEntity, IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, provide payment mode.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? CashId { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide opening date.")]
        public DateTime OpeningDate { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public Bank? Bank { get; set; }
        public Cash? Cash { get; set; }
        public EnumTypeCollection PaymentMode { get; set; }
    }
}