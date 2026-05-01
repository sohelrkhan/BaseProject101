namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class GroupedCardTableExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        private readonly string _groupByProperty;
        private readonly bool _needCalculation;
        private readonly bool _isGrouped;

        public GroupedCardTableExcelService(string groupByProperty, bool needCalculation = false, bool isGrouped = false)
        {
            _groupByProperty = groupByProperty;
            _needCalculation = needCalculation;
            _isGrouped = isGrouped;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var allProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var groupProp = allProps.FirstOrDefault(p => p.Name.Equals(_groupByProperty, StringComparison.OrdinalIgnoreCase));

            if (groupProp == null)
                throw new InvalidOperationException($"Property '{_groupByProperty}' not found.");

            var grouped = dataList
                .Where(x => groupProp.GetValue(x) != null)
                .GroupBy(x => groupProp.GetValue(x)?.ToString())
                .OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                var first = group.First();
                var cardProps = allProps
                    .Where(p => Attribute.IsDefined(p, typeof(ExcludeFromTableAttribute)) &&
                                !p.Name.Equals(_groupByProperty, StringComparison.OrdinalIgnoreCase))
                    .ToList();

               var  tablePropsLength = props
                        .Where(p =>
                            p.Name != "Code" &&
                            (p.GetCustomAttribute<ExcludeFromTableAttribute>() == null))
                        .ToArray().Length;

                int colIndex = 1;

                // 🔷 Group Header Card Title
                ws.Cell(row, 1).Value = $"{_groupByProperty}: {group.Key}";
                ws.Range(row, 1, row, tablePropsLength).Merge().Style.Font.FontSize = 12;
                ws.Range(row, 1, row, tablePropsLength).Style.Font.Italic = true;
                ws.Range(row, 1, row, tablePropsLength).Style.Font.FontColor = XLColor.White;
                ws.Range(row, 1, row, tablePropsLength).Style.Fill.SetBackgroundColor(XLColor.Gray);
                row++;

                // 🔷 Card Details: 3 items per row, key and value in same cell
                int itemsPerRow = 3;
                int currentColumn = 1;
                int currentCardRow = row;
                int itemsInCurrentRow = 0;

                foreach (var prop in cardProps)
                {
                    var label = SplitCamelCase(prop.Name);
                    var value = prop.GetValue(first)?.ToString() ?? "N/A";

                    ws.Cell(currentCardRow, currentColumn).Value = $"{label}: {value}";
                    ws.Cell(currentCardRow, currentColumn).Style.Fill.BackgroundColor = XLColor.White;
                    ws.Cell(currentCardRow, currentColumn).Style.Font.SetBold().Font.FontSize = 10;
                    ws.Cell(currentCardRow, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(currentCardRow, currentColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    currentColumn++;
                    itemsInCurrentRow++;

                    if (itemsInCurrentRow == itemsPerRow)
                    {
                        currentCardRow++;
                        currentColumn = 1;
                        itemsInCurrentRow = 0;
                    }
                }

                // Adjust row for next section
                row = currentCardRow + 1;


                var standard = new StandardTableExcelService<T, F>(_needCalculation, _isGrouped, _groupByProperty)
                {
                    ws = this.ws,
                    row = this.row
                };
                standard.ComposeTable(group.ToList());
                row = standard.row + 2;
            }
        }

        private string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "(\\B[A-Z])", " $1");
        }
    }
}
