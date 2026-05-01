namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<PaginatedResponse<Employee>> GetEmployeesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<IQueryable<Employee>> GetAllEmployeeFilterByIdQuery(int? companyId, int? statusId);
        Task<string> GetEmployeeNameByEmployeeIdsAsync(int[] employeeIds);
        Task<ICollection<SelectModel>> EmployeeSelectListAsync(CancellationToken cancellationToken);
        Task<ICollection<SelectModel>> EmployeeSelectListByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
        Task<bool> IsEmailExistAsync(string email); 
    }
}