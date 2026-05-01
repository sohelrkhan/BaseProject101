using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardIncomeExpenseExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        private readonly Dictionary<string, string> _incomeHeaders;
        private readonly Dictionary<string, string> _expenseHeaders;
        private readonly bool _needCalculation;

        public StandardIncomeExpenseExcelService(
            Dictionary<string, string> incomeHeaders,
            Dictionary<string, string> expenseHeaders,
            bool needCalculation)
        {
            _incomeHeaders = incomeHeaders;
            _expenseHeaders = expenseHeaders;
            _needCalculation = needCalculation;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var data = dataList.ToList();

            var incomeData = data
                .Where(x => GetPropertyValue(x, "SourceType")?.ToString() == "Income")
                .ToList();

            var expenseData = data
                .Where(x => GetPropertyValue(x, "SourceType")?.ToString() != "Income")
                .ToList();

            if (incomeData.Any())
            {
                row = WriteSection(incomeData, _incomeHeaders, "INCOME SUMMARY", row);
                row += 2;
            }

            if (expenseData.Any())
            {
                row = WriteSection(expenseData, _expenseHeaders, "EXPENSE SUMMARY", row);
            }
        }

        private int WriteSection(List<T> items, Dictionary<string, string> headers, string title, int startRow)
        {
            int currentRow = startRow;

            // ===== Title =====
            ws.Cell(currentRow, 1).Value = title;
            ws.Cell(currentRow, 1).Style.Font.Bold = true;
            ws.Cell(currentRow, 1).Style.Font.FontSize = 14;
            currentRow++;

            // ===== Header =====
            int colIndex = 1;
            ws.Cell(currentRow, colIndex++).Value = "SL";

            foreach (var header in headers.Values)
            {
                ws.Cell(currentRow, colIndex++).Value = header;
            }

            ws.Range(currentRow, 1, currentRow, headers.Count + 1).Style.Font.Bold = true;
            ws.Range(currentRow, 1, currentRow, headers.Count + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            // ===== Data Rows =====
            int sl = 0;
            decimal totalAmount = 0;

            foreach (var item in items)
            {
                sl++;
                colIndex = 1;

                ws.Cell(currentRow, colIndex++).Value = sl;

                foreach (var propKey in headers.Keys)
                {
                    var value = GetPropertyValue(item, propKey);

                    if (value == null)
                    {
                        ws.Cell(currentRow, colIndex++).Value = "";
                        continue;
                    }

                    if (value is DateTime dt)
                        ws.Cell(currentRow, colIndex++).Value = dt.ToString("dd MMMM yyyy");
                    else if (value is decimal dec)
                    {
                        ws.Cell(currentRow, colIndex++).Value = dec;
                        totalAmount += dec;
                    }
                    else if (value is int i)
                        ws.Cell(currentRow, colIndex++).Value = i;
                    else
                        ws.Cell(currentRow, colIndex++).Value = value.ToString();
                }

                currentRow++;
            }

            // ===== Total Row =====
            if (_needCalculation && headers.Count > 0)
            {
                colIndex = 1;
                ws.Cell(currentRow, colIndex++).Value = "Total";

                for (int i = 1; i < headers.Count; i++)
                    ws.Cell(currentRow, colIndex++).Value = "";

                ws.Cell(currentRow, colIndex).Value = totalAmount;
                ws.Row(currentRow).Style.Font.Bold = true;

                currentRow++;
            }

            return currentRow;
        }

        private object? GetPropertyValue(T item, string propertyName)
        {
            var prop = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return prop?.GetValue(item);
        }
    }
}
