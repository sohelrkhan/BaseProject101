namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(DatabaseContext context) : base(context) { }

        public async Task<ICollection<Event>> GetEventsByAccountUnitIdAsync(int accountUnitId)
        {
            var events = await dbContext.Events
                .AsNoTracking()
                .Where(e => e.AccountUnitId == accountUnitId && !e.IsDeleted)
                .ToListAsync();

            return events;
        }

        public override async Task<Event?> GetByIdAsync(int id)
        {
            var entity = await dbContext.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            return entity;
        }

        public async Task<PaginatedResponse<Event>> GetEventsFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get events
            var events = dbContext.Events.AsNoTracking()
                .Where(a => a.AccountUnitId == paginationRequest.AccountUnitId & !a.IsDeleted);

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                events = events.Where(a => (a.Name != null && EF.Functions.Like(a.Name, searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Event, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = u => u.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                events = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? events.OrderByDescending(sortExpression) : events.OrderBy(sortExpression);
            else
                events = events.OrderBy(u => u.Id);

            // Get total count before paging
            var totalCount = await events.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await events
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Event>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public async Task<IEnumerable<SelectModel>> GetEventSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken)
        {
            var events = await dbContext.Events
                .AsNoTracking()
                .Where(e => e.AccountUnitId == accountUnitId && !e.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync(cancellationToken);

            return events;
        }
    }
}