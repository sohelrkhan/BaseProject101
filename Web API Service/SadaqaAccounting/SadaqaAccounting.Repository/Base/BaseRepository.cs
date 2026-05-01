namespace SadaqaAccounting.Repository.Base
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DatabaseContext dbContext;

        protected BaseRepository(DatabaseContext databaseContext)
        {
            dbContext = databaseContext;
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            var getAllAsync = await dbContext.Set<T>().AsNoTracking().ToListAsync();
            return getAllAsync;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var getByIdAsync = await dbContext.Set<T>().FindAsync(id);
            return getByIdAsync;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
            var createAsync = await dbContext.SaveChangesAsync() > 0;

            return entity;
        }

        public virtual async Task<bool> CreateBulkAsync(IEnumerable<T> entities)
        {
            await dbContext.Set<T>().AddRangeAsync(entities);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> CreateBulkInsertAsync(IEnumerable<T> entities)
        {
            await dbContext.BulkInsertAsync(entities.ToList());
            return true; 
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var local = dbContext.Set<T>()
                .Local
                .FirstOrDefault(entry =>
                dbContext.Entry(entry).Property("Id").CurrentValue == dbContext.Entry(entity).Property("Id").CurrentValue);

            if (local != null)
                dbContext.Entry(local).State = EntityState.Detached;

            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            dbContext.Entry(entity).State = EntityState.Detached;

            return entity;
        }

        public virtual async Task<bool> UpdateBulkAsync(IEnumerable<T> entities)
        {
            dbContext.Set<T>().UpdateRange(entities);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> UpdateBulkInsertAsync(IEnumerable<T> entities)
        {
            await dbContext.BulkUpdateAsync(entities.ToList());
            return true;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            var deleteAsync = await dbContext.SaveChangesAsync() > 0;
            dbContext.Entry(entity).State = EntityState.Detached;

            return deleteAsync;
        }

        public virtual async Task<bool> DeleteBulkAsync(IEnumerable<T> entities)
        {
            dbContext.Set<T>().RemoveRange(entities);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public virtual Task CreateWithoutSaveAsync(T entity)
        {
            dbContext.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        public virtual Task CreateBulkWithoutSaveAsync(IEnumerable<T> entities)
        {
            dbContext.Set<T>().AddRange(entities);
            return Task.CompletedTask;
        }

        public virtual Task UpdateWithoutSaveAsync(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual Task DeleteWithoutSaveAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateBulkWithoutSaveAsync(IEnumerable<T> entities)
        {
            dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task<(ICollection<T> Data, int TotalRecords)> GetPaginatedAsync(int page, int pageSize, string? searchTerm = null, string? sortField = null, string sortOrder = "asc",
            Expression<Func<T, bool>>? additionalFilter = null, ICollection<AdditionalFilter>? additionalFilters = null)
        {
            var query = dbContext.Set<T>().AsQueryable();

            // Apply additional filter if provided
            if (additionalFilter != null)
                query = query.Where(additionalFilter);

            if (additionalFilters != null && additionalFilters.Count > 0)
                query = AdditionalFilterQuery(query, additionalFilters);

            // Apply search filter if implemented by derived class
            if (!string.IsNullOrEmpty(searchTerm))
                query = ApplySearchFilter(query, searchTerm);

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortField))
                query = ApplySorting(query, sortField, sortOrder);

            // Apply pagination
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        protected virtual IQueryable<T> ApplySearchFilter(IQueryable<T> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            // Get all string properties of the entity
            var stringProperties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                .ToList();

            if (!stringProperties.Any())
                return query;

            // Build dynamic OR condition for all string properties
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var property in stringProperties)
            {
                var propertyAccess = Expression.Property(parameter, property);
                var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchValue = Expression.Constant(searchTerm, typeof(string));
                var containsCall = Expression.Call(propertyAccess, containsMethod!, searchValue);
                var condition = Expression.AndAlso(nullCheck, containsCall);

                combinedExpression = combinedExpression == null
                    ? condition
                    : Expression.OrElse(combinedExpression, condition);
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        protected virtual IQueryable<T> ApplySorting(IQueryable<T> query, string sortField, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortField))
                return query;

            // Validate that the property exists
            var propertyInfo = typeof(T).GetProperty(sortField,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            if (propertyInfo == null)
                return query;

            // Use System.Linq.Dynamic.Core for dynamic sorting
            var direction = sortOrder?.ToLower() == "desc" ? "descending" : "ascending";
            query = query.OrderBy($"{propertyInfo.Name} {direction}");

            return query;
        }
        protected virtual IQueryable<T> AdditionalFilterQuery(IQueryable<T> query, ICollection<AdditionalFilter>? filters)
        {
            if (filters == null || !filters.Any())
                return query;

            foreach (var filter in filters)
            {
                if (string.IsNullOrEmpty(filter.FilterPropertyName) || filter.FilterValue == null)
                    continue;

                if (!PropertyExists<T>(filter.FilterPropertyName))
                    continue;

                var filterExpression = filter.Operator.ToLower() switch
                {
                    "contains" => $"{filter.FilterPropertyName}.Contains(@0)",
                    "startswith" => $"{filter.FilterPropertyName}.StartsWith(@0)",
                    "endswith" => $"{filter.FilterPropertyName}.EndsWith(@0)",
                    "!=" => $"{filter.FilterPropertyName} != @0",
                    ">" => $"{filter.FilterPropertyName} > @0",
                    "<" => $"{filter.FilterPropertyName} < @0",
                    ">=" => $"{filter.FilterPropertyName} >= @0",
                    "<=" => $"{filter.FilterPropertyName} <= @0",
                    _ => $"{filter.FilterPropertyName} == @0" // Default to equality
                };

                try
                {
                    query = query.Where(filterExpression, filter.FilterValue);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return query;
        }

        private static bool PropertyExists<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;

            var propertyInfo = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance);

            return propertyInfo != null;
        }
    }
}