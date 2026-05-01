namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class GroupedTablePdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly string _groupByProperty;

        public bool _needcalculation { get; set; }


        public GroupedTablePdfService(string groupByProperty,bool needcalculation = false)
        {
            _groupByProperty = groupByProperty;
            _needcalculation = needcalculation;
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string unused = null)
        {
            var properties = typeof(T).GetProperties();
            // Find the property first
            var groupProp = properties.FirstOrDefault(p => p.Name.Equals(_groupByProperty, StringComparison.OrdinalIgnoreCase));

            string displayName = groupProp?.Name ?? string.Empty; 

            if (groupProp != null)
            {
                var displayAttr = groupProp.GetCustomAttribute<DisplayHeaderAttribute>();
                if (displayAttr != null)
                {
                    displayName = displayAttr.HeaderText; 
                }
            }

            var groupedData = dataList
                .Where(x => groupProp.GetValue(x) != null)
                .GroupBy(x => groupProp.GetValue(x)?.ToString())
                .OrderBy(g => g.Key);

            container.Column(column =>
            {
                foreach (var group in groupedData)
                {
                    column.Item().PaddingTop(3).Background(Colors.Grey.Lighten1)
                          .Text($"{displayName} : {group.Key}").FontSize(9).Bold();

                    column.Item().Element(inner =>
                    {
                        var standardService = new StandardTablePdfService<T, F>(_needcalculation);
                        standardService.ComposeTable(inner, group.ToList(), _groupByProperty);
                    });
                }
            });
        }
    }
}
