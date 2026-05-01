namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class SummaryAndTableExcelService<T, F, H> : BaseExcelService<T, F> where F : IFilterItem
    {
        private readonly H _headerInfo;
        private readonly bool _needCalculation;

        public SummaryAndTableExcelService(H headerInfo, bool needCalculation = false)
        {
            _headerInfo = headerInfo;
            _needCalculation = needCalculation;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var headerProps = typeof(H).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var tableProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Dynamically calculate propsPerRow
            int totalLength = headerProps.Sum(p =>
            {
                var value = p.GetValue(_headerInfo)?.ToString()?.Length ?? 0;
                var labelLength = p.Name.Length;
                return labelLength + value;
            });

            double avgLength = (double)totalLength / headerProps.Length;

            int propsPerRow = avgLength switch
            {
                <= 20 => 7,
                <= 40 => 3,
                <= 60 => 2,
                _ => 1
            };

            int colIndex = 1;

            for (int i = 0; i < headerProps.Length; i += propsPerRow)
            {
                colIndex = 1;
                for (int j = i; j < i + propsPerRow && j < headerProps.Length; j++)
                {
                    var prop = headerProps[j];
                    string label = prop.Name.ToSentenceCase();
                    string value = GetValue(prop);

                    // Print in the format: Prop: Value
                    ws.Cell(row, colIndex).Value = $"  {label}  :   {value}";
                    ws.Cell(row, colIndex).Style.Font.SetBold();
                    colIndex++;
                }
                ws.Range(row, 1, row, tableProps.Length).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range(row, 1, row, tableProps.Length).Style.Border.BottomBorderColor = XLColor.Gray;
                ws.Range(row, 1, row, tableProps.Length).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Range(row, 1, row, tableProps.Length).Style.Font.SetBold().Font.FontSize = 11; 
                ws.Range(row, 1, row, tableProps.Length).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                row += 2;
            }

            row++; 

            var standard = new StandardTableExcelService<T, F>(_needCalculation);
            standard.ws = ws;
            standard.row = row;
            standard.ComposeTable(dataList);
            row = standard.row + 1;
        }

        private string GetValue(PropertyInfo prop)
        {
            var value = prop.GetValue(_headerInfo);
            return value switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm"),
                decimal d => d.ToString("N2"),
                bool b => b ? "Yes" : "No",
                _ => value?.ToString() ?? string.Empty
            };
        }
    }
}
