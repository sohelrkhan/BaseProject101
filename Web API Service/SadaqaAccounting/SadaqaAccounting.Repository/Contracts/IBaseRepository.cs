namespace SadaqaAccounting.Repository.Contracts
{
    public interface IBaseRepository<T> where T : class
    {
        Task<ICollection<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<bool> CreateBulkAsync(IEnumerable<T> entities);
        Task<bool> CreateBulkInsertAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<bool> UpdateBulkAsync(IEnumerable<T> entities);
        Task<bool> UpdateBulkInsertAsync(IEnumerable<T> entities);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteBulkAsync(IEnumerable<T> entities);
        Task CreateWithoutSaveAsync(T entity);
        Task CreateBulkWithoutSaveAsync(IEnumerable<T> entities);
        Task UpdateWithoutSaveAsync(T entity);
        Task UpdateBulkWithoutSaveAsync(IEnumerable<T> entities);
        Task DeleteWithoutSaveAsync(T entity);
        Task<(ICollection<T> Data, int TotalRecords)> GetPaginatedAsync(int page, int pageSize, string? searchTerm = null, string? sortField = null, string sortOrder = "asc",
            Expression<Func<T, bool>>? additionalFilter = null, ICollection<AdditionalFilter>? additionalFilters = null);
    }
}