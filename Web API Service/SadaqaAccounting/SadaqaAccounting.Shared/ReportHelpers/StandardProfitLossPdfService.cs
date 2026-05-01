namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardProfitLossPdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        public StandardProfitLossPdfService()
        {
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string groupByPropertyName = null)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var relevantProps = new[] { "AccountName", "Balance" };
            var dataListList = dataList.ToList();

            container.Table(table =>
            {
                // === Define Columns ===
                table.ColumnsDefinition(columns =>
                {
                    foreach (var prop in properties.Where(p => relevantProps.Contains(p.Name)))
                    {
                        columns.RelativeColumn();
                    }
                });

                // === Body Rows ===
                RenderProfitLossRows(table, dataListList, properties, relevantProps);

            });
        }

        private void RenderProfitLossRows<T>(
        TableDescriptor table,
        IEnumerable<T> items,
        PropertyInfo[] properties,
        string[] relevantProps,
        int level = 0)
        {
            foreach (var item in items)
            {
                foreach (var prop in properties.Where(p => relevantProps.Contains(p.Name)))
                {
                    string displayValue = "";
                    var value = prop.GetValue(item);
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    if (value != null)
                    {
                        if (prop.Name == "AccountName")
                        {
                            displayValue = new string(' ', level * 4) + value.ToString(); // Indentation
                        }
                        else if (prop.Name == "Balance")
                        {
                            decimal selfBalance = (decimal?)item.GetType().GetProperty("SelfBalance")?.GetValue(item) ?? 0;
                            decimal totalBalance = (decimal?)item.GetType().GetProperty("Balance")?.GetValue(item) ?? 0;
                            decimal childBalance = totalBalance - selfBalance;

                            string self = Math.Abs(selfBalance).ToString("N2", CultureInfo.InvariantCulture);
                            string child = Math.Abs(childBalance).ToString("N2", CultureInfo.InvariantCulture);

                            bool hasChildren = (item.GetType().GetProperty("Children")?.GetValue(item) as IEnumerable<T>)?.Any() ?? false;

                            displayValue = hasChildren
                                ? $"{self} + {child}"
                                : self;
                        }
                        else
                        {
                            displayValue = value.ToString();
                        }
                    }

                    bool isRightAligned = IsNumericType(type);
                    bool isNameColumn = prop.Name == "AccountName";
                   
                    table.Cell().Element(e =>
                    {
                        var parentAccount = item.GetType().GetProperty("ParentAccount")?.GetValue(item)?.ToString();

                        if (parentAccount == "Total" || parentAccount == "GrossNet")
                        {
                            if (isNameColumn)
                                return CellStyleGroupTotalLeft(e); // left-align for AccountName
                            else
                                return CellStyleGroupTotalRight(e); // right-align for balance
                        }

                        // Normal styles
                        if (isNameColumn)
                            return CellStyleIndented(e, level);
                        else if (isRightAligned)
                            return CellStyleRight(e);
                        else
                            return CellStyle(e);
                    })
                    .Text(displayValue)
                    .FontSize(8)
                    .FontFamily("Courier New");

                }

                // Recursive call for children
                var children = item.GetType().GetProperty("Children")?.GetValue(item) as IEnumerable<T>;
                if (children != null)
                {
                    RenderProfitLossRows(table, children, properties, relevantProps, level + 1);
                }
            }

            // === Helpers ===

            static IContainer CellStyle(IContainer container) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignLeft();

            static IContainer CellStyleRight(IContainer container) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignRight();

            static IContainer CellStyleOnlyTotal(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.LightGreen.Lighten1).Padding(5).AlignLeft().AlignRight();
            
            static IContainer CellStyleGroupTotalRight(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignLeft().AlignRight();
            
            static IContainer CellStyleGroupTotalLeft(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignLeft().AlignLeft();
            
            static IContainer CellStyleGroupTotalHead(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignMiddle().AlignLeft();

            static IContainer CellStyleHeader(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten2).Padding(3).AlignMiddle().AlignLeft();

            static IContainer CellStyleHeaderRight(IContainer container) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten2).Padding(3).AlignMiddle().AlignRight();

            static IContainer CellStyleIndented(IContainer container, int level) =>
                container
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .PaddingLeft(10 + (level * 10))
                    .PaddingVertical(5)
                    .AlignMiddle()
                    .AlignLeft();
        }
        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(decimal) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(long) || type == typeof(short);
        }
    }
}
