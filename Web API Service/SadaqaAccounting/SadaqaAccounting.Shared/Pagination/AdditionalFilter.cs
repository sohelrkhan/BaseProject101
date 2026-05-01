namespace SadaqaAccounting.Shared.Pagination
{
    public class AdditionalFilter
    {
        public object? FilterValue { get; set; }
        public string? FilterPropertyName { get; set; }
        public string Operator { get; set; } = "==";
    }
}