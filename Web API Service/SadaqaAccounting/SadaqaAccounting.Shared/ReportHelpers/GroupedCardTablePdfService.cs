namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class GroupedCardTablePdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly string _groupByProperty;
        public bool _needcalculation { get; set; }

        private readonly bool _isGrouped;

        public GroupedCardTablePdfService(string groupByProperty, bool needcalculation = false, bool isGrouped = false)
        {
            _groupByProperty = groupByProperty;
            _needcalculation = needcalculation;
            _isGrouped = isGrouped;
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string unused = null)
        {
            var groupProp = typeof(T).GetProperty(_groupByProperty, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (groupProp == null)
                throw new InvalidOperationException($"Property '{_groupByProperty}' not found on type {typeof(T).Name}");

            var groupedData = dataList
                .Where(x => groupProp.GetValue(x) != null)
                .GroupBy(x => groupProp.GetValue(x)?.ToString())
                .OrderBy(g => g.Key);

            container.Column(column =>
            {
                foreach (var group in groupedData)
                {
                    var firstItem = group.First();
                    column.Item().Element(header =>
                    {
                        RenderCardHeader(header, firstItem, _groupByProperty, group.Key);
                    });

                    column.Item().Element(inner =>
                    {
                        var standardService = new StandardTablePdfService<T, F>(_needcalculation, _isGrouped);
                        standardService.ComposeTable(inner, group.ToList(), _groupByProperty);
                    });

                    column.Item().PaddingBottom(15);
                }
            });
        }

        private void RenderCardHeader(IContainer container, T item, string groupByLabel, string groupKey)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Background(Colors.White)
                .Column(col =>
                {
                    col.Spacing(4);

                    // Header Title (Group Key)
                    col.Item().PaddingTop(4).PaddingLeft(5).Text($"{groupKey}")
                        .FontSize(10)
                        .SemiBold()
                        .FontColor(Colors.Black);

                    var rowItems = props
                    .Where(prop =>
                        prop.GetCustomAttribute<ExcludeFromTableAttribute>() != null &&
                        !prop.Name.Equals(groupByLabel, StringComparison.OrdinalIgnoreCase) &&
                        prop.GetValue(item) != null)
                    .Select(prop =>
                    {
                        string displayName = prop.Name.ToSentenceCase();
                        var value = prop.GetValue(item);
                        return $"{displayName}: {value}";
                    })
                    .ToList();

                    int itemsPerRow = 5; 
                    for (int i = 0; i < rowItems.Count; i += itemsPerRow)
                    {
                        var chunk = rowItems.Skip(i).Take(itemsPerRow).ToList();

                        col.Item().Row(row =>
                        {
                            foreach (var itemText in chunk)
                            {
                                row.AutoItem().Padding(6).Text(itemText)
                                    .FontSize(6).Bold().FontColor(Colors.Grey.Darken2);
                            }
                        });
                    }

                });
        }
    }
}
