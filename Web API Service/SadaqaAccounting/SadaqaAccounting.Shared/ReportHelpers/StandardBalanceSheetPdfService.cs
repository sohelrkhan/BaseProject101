namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardBalanceSheetPdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        public StandardBalanceSheetPdfService()
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
                for (int i = 0; i < dataListList.Count; i++)
                {
                    var item = dataListList[i];

                    foreach (var prop in properties.Where(p => relevantProps.Contains(p.Name)))
                    {
                        string displayValue = "";
                        var value = prop.GetValue(item);
                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (value != null)
                        {
                            if (prop.Name == "AccountName")
                            {
                                var treePrefix = item.GetType().GetProperty("TreePrefix")?.GetValue(item)?.ToString() ?? "";
                                displayValue = treePrefix + value.ToString();
                            }
                            else if (prop.Name == "Balance")
                            {
                                var selfBalance = (decimal?)item.GetType().GetProperty("SelfBalance")?.GetValue(item) ?? 0;
                                var totalBalance = (decimal?)item.GetType().GetProperty("Balance")?.GetValue(item) ?? 0;
                                var childBalance = totalBalance - selfBalance;

                                var formattedSelf = selfBalance < 0 ? Math.Abs(selfBalance) : selfBalance;

                                var formattedChild = childBalance < 0 ? Math.Abs(childBalance) : childBalance;

                                string self = formattedSelf.ToString("N2", CultureInfo.InvariantCulture); // => "110,100,000.00"
                                string child = formattedChild.ToString("N2", CultureInfo.InvariantCulture); // => "110,100,000.00"

                                var hasChildren = (bool?)item.GetType().GetProperty("HasChildren")?.GetValue(item) ?? false;

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
                            if (isNameColumn)
                            {
                                var level = (int?)item.GetType().GetProperty("Level")?.GetValue(item) ?? 0;
                                return CellStyleIndented(e, level);
                            }
                            else if (isRightAligned)
                                return CellStyleRight(e);
                            else
                                return CellStyle(e);
                        })
                        .Text(displayValue)
                        .FontSize(8)
                        .FontFamily("Courier New");
                    }

                    // === Subtotal Row ===
                    int currentLevel = (int?)item.GetType().GetProperty("Level")?.GetValue(item) ?? 0;

                    bool isLastInGroup = false;
                    if (currentLevel > 0 && i + 1 < dataListList.Count)
                    {
                        int nextLevel = (int?)dataListList[i + 1]?.GetType().GetProperty("Level")?.GetValue(dataListList[i + 1]) ?? 0;
                        isLastInGroup = nextLevel <= 0;
                    }
                    else if (currentLevel > 0 && i + 1 == dataListList.Count)
                    {
                        isLastInGroup = true;
                    }

                    if (isLastInGroup)
                    {
                        // Find parent
                        var parent = FindTopLevelParent(dataListList, i);
                        if (parent != null)
                        {
                            decimal total = CalculateGroupTotal(dataListList, parent);
                            string groupName = parent.GetType().GetProperty("AccountName")?.GetValue(parent)?.ToString();

                            var formattedTotal = total < 0 ? Math.Abs(total) : total;

                            string totalFormated = formattedTotal.ToString("N2", CultureInfo.InvariantCulture); // => "110,100,000.00"
                            foreach (var prop in properties.Where(p => relevantProps.Contains(p.Name)))
                            {
                                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                                if (prop.Name == "AccountName")
                                {

                                    table.Cell().Element(CellStyleOnlyTotal).Text($"Total {groupName}").FontSize(9).Bold();
                                }
                                else if (prop.Name == "Balance")
                                {
                                    // Render subtotal row with colspan = 2
                                    table.Cell().Element(CellStyleOnlyTotal)
                                        .Text(totalFormated)
                                        .FontSize(9)
                                        .Bold();
                                }
                            }
                        }
                    }
                }
                // === Final Row: Total Liabilities & Equity ===
                var liabilities = dataListList.FirstOrDefault(x =>
                    (x.GetType().GetProperty("Level")?.GetValue(x) as int?) == 0 &&
                    (x.GetType().GetProperty("AccountName")?.GetValue(x)?.ToString() == "Liabilities"));

                var equity = dataListList.FirstOrDefault(x =>
                    (x.GetType().GetProperty("Level")?.GetValue(x) as int?) == 0 &&
                    (x.GetType().GetProperty("AccountName")?.GetValue(x)?.ToString() == "Equity"));

                decimal liabilitiesTotal = liabilities != null ? CalculateGroupTotal(dataListList, liabilities) : 0;
                decimal equityTotal = equity != null ? CalculateGroupTotal(dataListList, equity) : 0;
                decimal finalTotal = liabilitiesTotal + equityTotal;

                var finalFormatted = finalTotal < 0 ? Math.Abs(finalTotal) : finalTotal;
                
                string formatedFinal = finalFormatted.ToString("N2", CultureInfo.InvariantCulture); // => "110,100,000.00"

                foreach (var prop in properties.Where(p => relevantProps.Contains(p.Name)))
                {
                    if (prop.Name == "AccountName")
                    {
                        table.Cell().Element(CellStyleOnlyTotal).Text("Total Liabilities & Equity").FontSize(9).Bold();
                    }
                    else if (prop.Name == "Balance")
                    {
                        table.Cell().Element(CellStyleOnlyTotal).Text(formatedFinal).FontSize(9).Bold();
                    }
                    else
                    {
                        table.Cell().Element(CellStyleOnlyTotal).Text(""); // empty cell
                    }
                }

            });

            // === Helpers ===


            // Helper to find top-level parent for a given index
            T FindTopLevelParent(List<T> list, int startIndex)
            {
                for (int j = startIndex; j >= 0; j--)
                {
                    var level = (int?)list[j]?.GetType().GetProperty("Level")?.GetValue(list[j]) ?? 0;
                    if (level == 0)
                        return list[j];
                }
                return default;
            }

            static IContainer CellStyle(IContainer container) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignLeft();

            static IContainer CellStyleRight(IContainer container) =>
                container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignRight();

            static IContainer CellStyleOnlyTotal(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.LightGreen.Lighten1).Padding(5).AlignMiddle().AlignRight();

            static IContainer CellStyleIndented(IContainer container, int level) =>
                container
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .PaddingLeft(10 + (level * 10))
                    .PaddingVertical(5)
                    .AlignMiddle()
                    .AlignLeft();

            static bool IsNumericType(Type type)
            {
                return type == typeof(byte) || type == typeof(short) || type == typeof(ushort)
                    || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
                    || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
            }
        }

        private decimal CalculateGroupTotal(List<T> dataList, T parentItem)
        {
            int parentId = (int?)parentItem.GetType().GetProperty("Id")?.GetValue(parentItem) ?? 0;

            decimal self = (decimal?)parentItem.GetType().GetProperty("SelfBalance")?.GetValue(parentItem) ?? 0;
            decimal total = self;

            var children = GetAllDescendants(dataList, parentId);

            foreach (var child in children)
            {
                decimal childBalance = (decimal?)child.GetType().GetProperty("SelfBalance")?.GetValue(child) ?? 0;
                total += childBalance;
            }

            return total;
        }

        private List<T> GetAllDescendants<T>(List<T> dataList, int parentId)
        {
            var result = new List<T>();
            var stack = new Stack<int>();
            stack.Push(parentId);

            while (stack.Count > 0)
            {
                int currentId = stack.Pop();
                var children = dataList.Where(x =>
                    (int?)x.GetType().GetProperty("ParentId")?.GetValue(x) == currentId).ToList();

                foreach (var child in children)
                {
                    result.Add(child);
                    int childId = (int?)child.GetType().GetProperty("Id")?.GetValue(child) ?? 0;
                    stack.Push(childId);
                }
            }

            return result;
        }
    }
}
