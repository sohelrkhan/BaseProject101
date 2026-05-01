namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("AccountUnits", Schema = "MasterSettings")]
    public class AccountUnit: IAuditableEntity, IDelatableEntity
    {
        public AccountUnit()
        {
            UserAccountUnits = new HashSet<UserAccountUnit>();
            Donors = new HashSet<Donor>();
            IncomeCategories = new HashSet<IncomeCategory>();
            Incomes = new HashSet<Income>();
            Events = new HashSet<Event>();
            Banks = new HashSet<Bank>();
            ExpenseCategories = new HashSet<ExpenseCategory>();
            Expenses = new HashSet<Expense>();
            CashLedgers = new HashSet<CashLedger>();
            BankLedgers = new HashSet<BankLedger>();
            Assets = new HashSet<Asset>();
            OpeningBalances = new HashSet<OpeningBalance>();
            Cashes = new HashSet<Cash>();
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
        public ICollection<Donor> Donors { get; set; }
        public ICollection<IncomeCategory> IncomeCategories { get; set; }
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Bank> Banks { get; set; }
        public ICollection<ExpenseCategory> ExpenseCategories { get; set; }
        public ICollection<Expense> Expenses { get; set; }
        public ICollection<CashLedger> CashLedgers { get; set; }
        public ICollection<BankLedger> BankLedgers { get; set; }
        public ICollection<Asset> Assets { get; set; }
        public ICollection<OpeningBalance> OpeningBalances { get; set; }
        public ICollection<Cash> Cashes { get; set; }
    }
}