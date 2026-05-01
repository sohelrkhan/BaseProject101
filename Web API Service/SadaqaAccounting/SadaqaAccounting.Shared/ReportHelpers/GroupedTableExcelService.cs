namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class GroupedTableExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        private readonly string _groupByProperty;

        public bool _needcalculation { get; set; }


        public GroupedTableExcelService(string groupByProperty,bool needcalculation=false)
        {
            _groupByProperty = groupByProperty;
            _needcalculation = needcalculation;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var props = typeof(T).GetProperties();

            var filteredProps = props
              .Where(p => !Attribute.IsDefined(p, typeof(ExcludeFromTableAttribute)))
              .ToArray();

            var propsLength = filteredProps.Length;

            var groupProp = props.FirstOrDefault(p => p.Name.Equals(_groupByProperty, StringComparison.OrdinalIgnoreCase));


            string displayName = groupProp?.Name ?? string.Empty;

            if (groupProp != null)
            {
                var displayAttr = groupProp.GetCustomAttribute<DisplayHeaderAttribute>();
                if (displayAttr != null)
                {
                    displayName = displayAttr.HeaderText;
                }
            }

            var grouped = dataList
                .Where(x => groupProp.GetValue(x) != null)
                .GroupBy(x => groupProp.GetValue(x).ToString())
                .OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                ws.Cell(row, 1).Value = $"{displayName}: {group.Key}";
                ws.Range(row, 1, row, propsLength).Merge().Style.Font.FontSize = 12;
                ws.Range(row, 1, row, propsLength).Style.Font.Italic = true;
                ws.Range(row, 1, row, propsLength).Style.Font.FontColor = XLColor.White;
                ws.Range(row, 1, row, propsLength).Style.Fill.SetBackgroundColor(XLColor.Gray);
                row++;

                var standard = new StandardTableExcelService<T, F>(_needcalculation);
                standard.ws = ws;
                standard.row = row;
                standard.ComposeTable(group.ToList());
                row = standard.row + 1;
            }
        }
    }

}
