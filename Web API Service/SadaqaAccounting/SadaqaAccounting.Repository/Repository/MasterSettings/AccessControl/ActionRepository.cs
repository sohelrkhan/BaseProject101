namespace SadaqaAccounting.Repository.Repository.MasterSettings.AccessControl
{
    public class ActionRepository : BaseRepository<Action>, IActionRepository
    {
        public ActionRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Action>> GetAllAsync()
        {
            var actions = await dbContext.Actions
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(x => x.Status)
                .ToListAsync();

            return actions;
        }

        public async Task<PaginatedResponse<Action>> GetActionsFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get actions
            var getActions = dbContext.Actions
                .Include(a => a.Status)
                .Where(a => !a.IsDeleted)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                getActions = getActions.Where(a => (a.Name != null && EF.Functions.Like(a.Name, searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Action, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = u => u.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                getActions = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? getActions.OrderByDescending(sortExpression) : getActions.OrderBy(sortExpression);
            else
                getActions = getActions.OrderBy(u => u.Id);

            // Get total count before paging
            var totalCount = await getActions.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await getActions
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Action>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public override async Task<Action?> GetByIdAsync(int id)
        {
            var action = await dbContext.Actions
                         .Where(x => !x.IsDeleted & x.Id == id)
                         .FirstOrDefaultAsync();

            return action;
        }

        public async Task<Action?> GetActionByName(string name)
        {
            var action = await dbContext.Actions
                .Where(a => a.Name.ToLower() == name.ToLower() && !a.IsDeleted)
                .FirstOrDefaultAsync();

            return action;
        }
    }
}