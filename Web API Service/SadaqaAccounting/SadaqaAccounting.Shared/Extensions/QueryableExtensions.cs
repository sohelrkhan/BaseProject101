namespace SadaqaAccounting.Shared.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyAdditionalFilters<T>(this IQueryable<T> query, ICollection<AdditionalFilter>? filters)
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