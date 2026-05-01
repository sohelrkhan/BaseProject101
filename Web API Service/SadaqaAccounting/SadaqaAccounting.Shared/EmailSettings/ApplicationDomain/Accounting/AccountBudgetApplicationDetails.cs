namespace SadaqaAccounting.Shared.EmailSettings.ApplicationDomain.Accounting
{
    public class AccountBudgetApplicationDetails : IEmailFeatureDetails
    {
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}