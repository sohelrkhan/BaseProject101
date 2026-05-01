namespace SadaqaAccounting.Shared.EmailSettings.Generator.Accounting
{
    public class AccountBudgetApplicationBodyGenerator : IEmailBodyGenerator
    {
        public string GenerateBody(ApplicationInformation info)
        {
            var data = (AccountBudgetApplicationDetails)info.EmailFeatureDetails;

            return $@"
            <table class='details-table'>
                <tr>
                    <th>Company</th>
                    <th>Name</th>
                    <th>Department</th>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Total Amount</th>
                </tr>
                <tr>
                    <td>{data.CompanyName}</td>
                    <td>{data.Name}</td>
                    <td>{data.DepartmentName}</td>
                    <td>{data.StartDate:dd MMM yyyy}</td>
                    <td>{data.EndDate:dd MMM yyyy}</td>
                    <td>{data.TotalAmount}</td>                   
                </tr>
            </table>";
        }
    }
}