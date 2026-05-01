namespace SadaqaAccounting.Shared.ErrorMessages
{
    public static class ProvideErrorMessage
    {
        // For User
        public readonly static string UserNotAuthenticated = "User is not authenticated.";
        public readonly static string UserNotFound = "User is not found! Please, try again.";
        public readonly static string canNotDeleteAsLeaveGroupAlreadyAssigned = "Can Not Delete As LeaveGroup Already Assigned";

        // For Action
        public readonly static string ActionIdNotFound = "Action cannot found! Please, provide valid id.";
        public readonly static string ActioneNotFound = "Action cannot found! Please, try again.";

        // For Module
        public readonly static string ModuleIdNotFound = "Module cannot found! Please, provide valid id.";
        public readonly static string ModuleNotFound = "Module cannot found! Please, try again.";

        // For Feature
        public readonly static string FeatureIdNotFound = "Feature cannot found! Please, provide valid id.";
        public readonly static string FeatureNotFound = "Feature cannot found! Please, try again.";

        // For Notification
        public readonly static string NotificationIdNotFound = "Notification cannot found! Please, provide valid id.";
        public readonly static string NotificationNotFound = "Notification cannot found! Please, try again.";

        // For Notification
        public readonly static string ReportRegistryIdNotFound = "ReportRegistry cannot found! Please, provide valid id.";
        public readonly static string ReportRegistryNotFound = "ReportRegistry cannot found! Please, try again.";

        // For Company
        public readonly static string CompanyIdNotFound = "Company cannot found! Please, provide valid id.";
        public readonly static string CompanyNotFound = "Company cannot found! Please, try again.";

        // For Employee
        public readonly static string EmployeeIdNotFound = "Employee cannot found! Please, provide valid id.";
        public readonly static string EmployeeNotFound = "Employee cannot found! Please, try again.";

        // For Expense
        public readonly static string ExpenseCategoryIdNotFound = "Expense Category cannot found! Please, provide valid id.";
        public readonly static string ExpenseCategoryNotFound = "Expense Category cannot found! Please, try again.";
        public readonly static string ExpenseIdNotFound = "Expense cannot found! Please, provide valid id.";
        public readonly static string ExpenseNotFound = "Expense cannot found! Please, try again.";

        // For Income
        public readonly static string IncomeCategoryIdNotFound = "Income Category cannot found! Please, provide valid id.";
        public readonly static string IncomeCategoryNotFound = "Income Category cannot found! Please, try again.";
        public readonly static string IncomeIdNotFound = "Income cannot found! Please, provide valid id.";
        public readonly static string IncomeNotFound = "Income cannot found! Please, try again.";
        
        // For Event
        public readonly static string EventIdNotFound = "Event cannot found! Please, provide valid id.";
        public readonly static string EventNotFound = "Event cannot found! Please, try again.";
        
        // For Donor
        public readonly static string DonorIdNotFound = "Donor cannot found! Please, provide valid id.";
        public readonly static string DonorNotFound = "Donor cannot found! Please, try again.";
        public readonly static string DuplicateDonor = "This donor already exist! Please, try again with different donor.";


        // For Asset
        public readonly static string AssetIdNotFound = "Asset cannot found! Please, provide valid id.";
        public readonly static string AssetnotFound = "Asset cannot found! Please, try again.";
        public readonly static string DuplicateAsset = "This asset already exist! Please, try again with different asset.";
        
        // For Bank
        public readonly static string BankIdNotFound = "Bank cannot found! Please, provide valid id.";
        public readonly static string BankNotFound = "Bank cannot found! Please, try again.";
        public readonly static string DuplicateBank = "This bank already exist! Please, try again with different donor.";

        // For Opening Balance
        public readonly static string OpeningBalanceIdNotFound = "Opening Balance cannot found! Please, provide valid id.";
        public readonly static string OpeningBalanceNotFound = "Opening Balance cannot found! Please, try again.";
    }
}