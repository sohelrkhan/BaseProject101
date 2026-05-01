namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardBalanceSheetTableExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        public StandardBalanceSheetTableExcelService()
        {
            
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var relevantProps = new[] { "AccountName", "Balance" };
            int col = 1;
            var headerRow = row;
            row++;
            int startDataRow = row;

            if (typeof(T).Name == "BalanceSheetExportGridModel")
            {
                // Cast to dynamic list
                var list = dataList.Cast<object>().ToList();
                var idProp = typeof(T).GetProperty("Id");
                var parentIdProp = typeof(T).GetProperty("ParentId");
                var selfBalanceProp = typeof(T).GetProperty("SelfBalance");
                var balanceProp = typeof(T).GetProperty("Balance");
                var accountNameProp = typeof(T).GetProperty("AccountName");
                var hasChildrenProp = typeof(T).GetProperty("HasChildren");

                var topLevelItems = list
                    .Where(x => (int)(parentIdProp?.GetValue(x) ?? 0) == 0)
                    .ToList();

                foreach (var parent in topLevelItems)
                {
                    int parentId = (int)idProp.GetValue(parent);
                    string groupName = accountNameProp.GetValue(parent)?.ToString();

                    // Get all children + self for this group
                    var groupItems = list
                        .Where(x => (int)idProp.GetValue(x) == parentId || IsDescendantOfDynamic(x, parentId, idProp, parentIdProp, list))
                        .ToList();

                    // Render each row
                    foreach (var item in groupItems)
                    {
                        var accountName = accountNameProp?.GetValue(item)?.ToString();
                        var selfBalance = (decimal?)selfBalanceProp?.GetValue(item) ?? 0;
                        var balance = (decimal?)balanceProp?.GetValue(item) ?? 0;
                        var hasChildren = (bool?)hasChildrenProp?.GetValue(item) ?? false;

                        decimal balanceDisplay = hasChildren
                            ? selfBalance + balance
                            : selfBalance;

                        ws.Cell(row, 1).Value = $"- {accountName}";
                        ws.Cell(row, 2).Value = balanceDisplay;
                        ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                        row++;
                    }

                    // Subtotal
                    decimal subtotalSelf = groupItems.Sum(x => (decimal?)selfBalanceProp?.GetValue(x) ?? 0);

                    ws.Cell(row, 1).Value = $"Total {groupName}";
                    ws.Range(row, 1, row, relevantProps.Length - 1).Merge();
                    ws.Cell(row, relevantProps.Length).Value = subtotalSelf;
                    ws.Cell(row, relevantProps.Length).Style.NumberFormat.Format = "#,##0.00";

                    ws.Range(row, 1, row, relevantProps.Length).Style.Font.Bold = true;
                    ws.Range(row, 1, row, relevantProps.Length).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Range(row, 1, row, relevantProps.Length).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, relevantProps.Length).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    row++;
                }

                // === Liabilities & Equity Total ===
                var liabEquityRootNames = new[] { "Liabilities", "Equity" };

                var liabEquityRoots = list
                    .Where(x =>
                    {
                        var name = accountNameProp?.GetValue(x)?.ToString()?.Trim();
                        return liabEquityRootNames.Contains(name, StringComparer.OrdinalIgnoreCase);
                    })
                    .ToList();

                var liabEquityItems = new List<object>();

                foreach (var root in liabEquityRoots)
                {
                    int rootId = (int)idProp.GetValue(root);
                    liabEquityItems.Add(root);
                }

                // Sum only Balance (not SelfBalance)
                decimal totalLiabEquity = liabEquityItems.Sum(x => (decimal?)balanceProp?.GetValue(x) ?? 0);

                // Output final row
                ws.Cell(row, 1).Value = "Total Liabilities & Equity";
                ws.Cell(row, 2).Value = totalLiabEquity;
                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";

                ws.Range(row, 1, row, 2).Style.Font.Bold = true;
                ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(row, 1, row, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                row++;

            }
            else
            {
                // Fallback: normal rendering
                foreach (var item in dataList)
                {
                    col = 1;
                    foreach (var prop in props.Where(p => relevantProps.Contains(p.Name)))
                    {
                        var value = prop.GetValue(item);
                        WriteCell(row, col, value);
                        col++;
                    }
                    row++;
                }
            }
        }

        private void WriteCell(int r, int c, object value)
        {
            if (value is DateTime dt)
                ws.Cell(r, c).Value = dt;
            else if (value is int i)
                ws.Cell(r, c).Value = i;
            else if (value is decimal d)
            {
                ws.Cell(r, c).Value = Math.Round(d, 2);
                ws.Cell(r, c).Style.NumberFormat.Format = "#,##0.00";
            }
            else if (value is double db)
            {
                ws.Cell(r, c).Value = Math.Round(db, 2);
                ws.Cell(r, c).Style.NumberFormat.Format = "#,##0.00";
            }
            else if (value is float f)
            {
                ws.Cell(r, c).Value = Math.Round(f, 2);
                ws.Cell(r, c).Style.NumberFormat.Format = "#,##0.00";
            }
            else if (value is long l)
                ws.Cell(r, c).Value = l;
            else if (value != null)
                ws.Cell(r, c).Value = value.ToString();
            else
                ws.Cell(r, c).Value = "";

            ws.Cell(r, c).Style.Font.FontSize = 9;
            ws.Cell(r, c).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Cell(r, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            if (value is not string && value is not DateTime)
                ws.Cell(r, c).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            else
                ws.Cell(r, c).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        }
        private bool IsDescendantOfDynamic(object item, int parentId, PropertyInfo idProp, PropertyInfo parentIdProp, List<object> list)
        {
            var current = item;
            while (true)
            {
                int currParentId = (int)(parentIdProp?.GetValue(current) ?? 0);
                if (currParentId == 0) return false;
                if (currParentId == parentId) return true;

                current = list.FirstOrDefault(x => (int)idProp.GetValue(x) == currParentId);
                if (current == null) break;
            }
            return false;
        }
    }
}
