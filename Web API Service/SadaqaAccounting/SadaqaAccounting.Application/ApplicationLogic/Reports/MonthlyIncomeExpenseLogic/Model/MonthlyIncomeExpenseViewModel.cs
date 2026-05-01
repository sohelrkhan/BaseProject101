using SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Model;

namespace SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Model
{
    public class MonthlyIncomeExpenseViewModel
    {
    }

    public class MonthlyIncomeExpenseGridModel
    {
        public string PreparedByName { get; set; }
        public string? Period { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public ICollection<ExpenseGridModel> ExpenseList { get; set; }
        public ICollection<IncomeGridModel> IncomeList { get; set; }
    }
    public class MonthlyIncomeExpenseRequestModel
    {
        public string? DateRange { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? AccountUnitId { get; set; }
        public int? EventId { get; set; }
    }
    public class MonthlyIncomeExpenseSummaryModel
    {
        public int SL { get; set; }
        public string? ExpenseCategoryName { get; set; }
        public string? EventName { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public decimal? ExpenseAmount { get; set; } = 0;
        [ExcludeFromTable]
        public string SourceType { get; set; }
        public string? IncomeCategoryName { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public decimal? IncomeAmount { get; set; }
    }
    public class MonthlyIncomeExpenseExcelModel
    {
        public int SL { get; set; }
        [ExcludeFromTable]
        public string? ExpenseCategoryName { get; set; }
        [ExcludeFromTable]
        public string? EventName { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public decimal? ExpenseAmount { get; set; } = 0;
        [ExcludeFromTable]
        public string SourceType { get; set; }
        [ExcludeFromTable]
        public string? IncomeCategoryName { get; set; }
        [ExcludeFromTable]
        public DateTime? ReceiptDate { get; set; }
        public decimal? IncomeAmount { get; set; }
    }
    public class ExportMonthlyIncomeExpenseReportModel
    {
        public string? ExportFileName { get; set; }
        public string? ReportName { get; set; }
        public string? Duration { get; set; }
        public int? EventId { get; set; }
        public string? ReportType { get; set; }
        public int? CompanyId { get; set; }
        public string? QrCodeUrl { get; set; }
        public MonthlyIncomeExpenseGridModel GridModel { get; set; }
        public ICollection<ReportFilter>? Filters { get; set; }
    }
    public class ReportFilter : IFilterItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
