namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardMonthWiseProfitLossPdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        public ICollection<string> _tableHeader { get; set; }
        public StandardMonthWiseProfitLossPdfService(ICollection<string> tableHeader)
        {
            _tableHeader = tableHeader;
        }
        public override void ComposeTable(IContainer container, ICollection<T> dataList, string groupByPropertyName = null)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var relevantProps = new List<string>();
            var dataListList = dataList.ToList();

            container.Table(table =>
            {
                // === Define Columns ===
                table.ColumnsDefinition(columns =>
                {
                    relevantProps.Add("Ordinary Income/Expense");
                    foreach (var prop in _tableHeader)
                    {
                        relevantProps.Add(prop);
                    }
                    relevantProps.Add("Total");

                    int n = 1;
                    foreach (var prop in relevantProps)
                    {
                        if (n == 1)
                        {
                            columns.RelativeColumn(3);
                        }
                        else
                        {
                            columns.RelativeColumn(1);
                        }

                        n++;
                    }
                });

                table.Header(header =>
                {
                    header.Cell()
                          .Element(CellStyleHeader)
                          .Text("Ordinary Income/Expense")
                          .SemiBold()
                          .FontSize(7);

                    foreach (var prop in _tableHeader)
                    {
                        bool isNumeric = true;
                        var headerText = prop;
                        header.Cell()
                              .Element(isNumeric ? CellStyleHeaderRight : CellStyleHeader)
                              .Text(headerText)
                              .SemiBold()
                              .FontSize(7);
                    }
                    header.Cell()
                         .Element(CellStyleHeaderRight)
                         .Text("Total")
                         .SemiBold()
                         .FontSize(7);
                });


                // === Body Rows ===
                RenderProfitLossRows(table, dataListList, properties, relevantProps);

                static IContainer CellStyleHeader(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten2).Padding(1).AlignMiddle().AlignLeft();

                static IContainer CellStyleHeaderRight(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten2).Padding(1).AlignMiddle().AlignRight();
            });
        }

        private void RenderProfitLossRows<T>(
        TableDescriptor table,
        IEnumerable<T> items,
        PropertyInfo[] properties,
        List<string> relevantProps,
        int level = 0)
        {
            foreach (var item in items)
            {
                foreach (var teamName in relevantProps)
                {
                    string displayValue = "";
                    var parentAccount = item.GetType().GetProperty("ParentAccount")?.GetValue(item)?.ToString();

                    if (teamName == "Ordinary Income/Expense")
                    {
                        var accountName = item.GetType().GetProperty("AccountName")?.GetValue(item)?.ToString();
                        displayValue = new string(' ', level * 4) + accountName;
                    }
                    else if (teamName == "Total")
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
                        var monthBalances = item.GetType().GetProperty("MonthBalances")?.GetValue(item) as Dictionary<string, decimal>;
                        var selfBalance = item.GetType().GetProperty("SelfBalance")?.GetValue(item) as decimal? ?? 0;
                        var totalBalance = item.GetType().GetProperty("Balance")?.GetValue(item) as decimal? ?? 0;
                        var childBalance = totalBalance - selfBalance;

                        decimal teamTotal = 0;
                        decimal teamSelf = 0;

                        if (monthBalances != null && monthBalances.TryGetValue(teamName, out var balance))
                        {
                            // Optional: If you want to separate self from total per team, you must store per-team self vs child split.
                            teamTotal = balance;
                        }

                        displayValue = Math.Abs(teamTotal).ToString("N2", CultureInfo.InvariantCulture);
                    }

                    bool isNameColumn = teamName == "Ordinary Income/Expense";

                    table.Cell().Element(e =>
                    {
                        if (parentAccount == "Total" || parentAccount == "GrossNet")
                        {
                            if (isNameColumn)
                                return CellStyleGroupTotalLeft(e);
                            else
                                return CellStyleGroupTotalRight(e);
                        }

                        if (isNameColumn)
                            return CellStyleIndented(e, level);
                        else
                            return CellStyleRight(e);
                    })
                    .Text(displayValue)
                    .FontSize(6)
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

            static IContainer CellStyleRight(IContainer container) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignRight();

            static IContainer CellStyleGroupTotalRight(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignLeft().AlignRight();

            static IContainer CellStyleGroupTotalLeft(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignLeft().AlignLeft();

            static IContainer CellStyleIndented(IContainer container, int level) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).PaddingLeft(5 + (level * 5)).PaddingVertical(5).AlignMiddle().AlignLeft();
        }
    }
}
