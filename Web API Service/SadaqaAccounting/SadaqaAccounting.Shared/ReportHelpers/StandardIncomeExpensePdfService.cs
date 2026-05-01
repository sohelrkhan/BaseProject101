using DocumentFormat.OpenXml.Spreadsheet;

namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardIncomeExpensePdfService<T, F>
     : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly Dictionary<string, string> _incomeHeaders;
        private readonly Dictionary<string, string> _expenseHeaders;
        private readonly bool _needCalculation;

        public StandardIncomeExpensePdfService(
            Dictionary<string, string> incomeHeaders,
            Dictionary<string, string> expenseHeaders,
            bool needCalculation)
        {
            _incomeHeaders = incomeHeaders;
            _expenseHeaders = expenseHeaders;
            _needCalculation = needCalculation;
        }

        public override void ComposeTable(
            IContainer container,
            ICollection<T> dataList,
            string groupByPropertyName = null)
        {
            var data = dataList.ToList();

            var incomeData = data
                .Where(x => x!.GetType()
                .GetProperty("SourceType")?
                .GetValue(x)?.ToString() == "Income")
                .ToList();

            var expenseData = data
                .Where(x => x!.GetType()
                .GetProperty("SourceType")?
                .GetValue(x)?.ToString() != "Income")
                .ToList();

            // 🔥 IMPORTANT: Only ONE child assigned
            container.Column(column =>
            {
                if (incomeData.Any())
                {
                    column.Item().Element(c =>
                        RenderTable(c, incomeData, _incomeHeaders, "INCOME SUMMARY"));
                }

                if (expenseData.Any())
                {
                    column.Item().PaddingTop(15).Element(c =>
                        RenderTable(c, expenseData, _expenseHeaders, "EXPENSE SUMMARY"));
                }
            });
        }

        // ============================================================
        // GENERIC TABLE RENDERER
        // ============================================================
        private void RenderTable(
            IContainer container,
            List<T> items,
            Dictionary<string, string> headers,
            string title)
        {
            container.Column(column =>
            {
                // ---------- Title ----------
                column.Item()
                      .PaddingBottom(5)
                      .Text(title)
                      .SemiBold()
                      .FontSize(10);

                // ---------- Table ----------
                column.Item().Table(table =>
                {
                    // Columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1); // SL

                        foreach (var _ in headers)
                            columns.RelativeColumn(2);
                    });

                    // Header Row
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle)
                              .Text("SL")
                              .SemiBold()
                              .FontSize(7);

                        foreach (var col in headers)
                        {
                            var propertyName = col.Value;
                            if (propertyName == "Amount")
                            {
                                header.Cell().Element(HeaderStyleRight)
                                      .Text(col.Value) // Display name
                                      .SemiBold()
                                      .FontSize(7);
                            }
                            else
                            {
                                header.Cell().Element(HeaderStyle)
                                      .Text(col.Value) // Display name
                                      .SemiBold()
                                      .FontSize(7);
                            }
                                
                        }
                    });

                    int sl = 0;
                    decimal totalAmount = 0;

                    foreach (var item in items)
                    {
                        sl++;

                        // SL
                        table.Cell()
                             .Element(CellStyle)
                             .Text(sl.ToString())
                             .FontSize(6);

                        foreach (var col in headers)
                        {
                            var propertyName = col.Key;
                            var prop = typeof(T).GetProperty(propertyName);

                            if (prop == null)
                            {
                                table.Cell().Element(CellStyle).Text("").FontSize(6);
                                continue;
                            }

                            var value = prop.GetValue(item);

                            string displayValue = "";

                            if (value != null)
                            {
                                if (value is DateTime dt)
                                    displayValue = dt.ToString("dd MMMM yyyy");

                                else if (value is decimal dec)
                                {
                                    displayValue = dec.ToString("N2", CultureInfo.InvariantCulture);
                                    totalAmount += dec;
                                }

                                else
                                    displayValue = value.ToString();
                            }

                            bool isNumeric = value is decimal or int or double or float;

                            table.Cell()
                                 .Element(isNumeric ? CellStyleRight : CellStyle)
                                 .Text(displayValue)
                                 .FontSize(6);
                        }
                    }

                    // ---------- Total Row ----------
                    if (_needCalculation && headers.Any())
                    {
                        // Empty cells except last numeric column
                        table.Cell()
                             .Element(TotalLabelStyle)
                             .Text("")
                             .SemiBold()
                             .FontSize(7);
                        table.Cell()
                             .Element(TotalLabelStyle)
                             .Text("")
                             .SemiBold()
                             .FontSize(7);
                        table.Cell()
                             .Element(TotalLabelStyle)
                             .Text("TOTAL")
                             .SemiBold()
                             .FontSize(7);

                        table.Cell()
                             .Element(TotalValueStyle)
                             .Text(totalAmount.ToString("N2", CultureInfo.InvariantCulture))
                             .SemiBold()
                             .FontSize(7);
                    }
                });
            });
        }

        // ============================================================
        // STYLES
        // ============================================================
        static IContainer HeaderStyle(IContainer container) =>
            container.Border(1)
                     .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten1)
                     .Background(QuestPDF.Helpers.Colors.Grey.Lighten2)
                     .Padding(3)
                     .AlignMiddle()
                     .AlignLeft();
        static IContainer HeaderStyleRight(IContainer container) =>
            container.Border(1)
                     .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten1)
                     .Background(QuestPDF.Helpers.Colors.Grey.Lighten2)
                     .Padding(3)
                     .AlignMiddle()
                     .AlignRight();

        static IContainer CellStyle(IContainer container) =>
            container.Border(1)
                     .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten1)
                     .Padding(5)
                     .AlignMiddle()
                     .AlignLeft();

        static IContainer CellStyleRight(IContainer container) =>
            container.Border(1)
                     .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten1)
                     .Padding(5)
                     .AlignMiddle()
                     .AlignRight();

        static IContainer TotalLabelStyle(IContainer container) =>
            container.Border(1)
                     .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten1)
                     .Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                     .Padding(5)
                     .AlignRight();

        static IContainer TotalValueStyle(IContainer container) =>
            container.Border(1)
                     .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten1)
                     .Background(QuestPDF.Helpers.Colors.LightGreen.Lighten2)
                     .Padding(5)
                     .AlignRight();
    }
}
