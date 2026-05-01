namespace SadaqaAccounting.Shared.Pagination
{
    public class PaginationRequest
    {
        public ICollection<AdditionalFilter>? AdditionalFilters { get; set; }
        public int? AccountUnitId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchTerm { get; set; }
    }
}