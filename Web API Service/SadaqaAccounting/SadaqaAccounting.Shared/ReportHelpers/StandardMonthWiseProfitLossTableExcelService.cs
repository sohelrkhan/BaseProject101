namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardMonthWiseProfitLossTableExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        public ICollection<string> _tableHeader { get; set; }
        public StandardMonthWiseProfitLossTableExcelService(ICollection<string> tableHeader)
        {
            _tableHeader = tableHeader;
        }
        public override void ComposeTable(ICollection<T> dataList)
        {
            int col = 1;

            // Header

            ws.Cell(row, col).Value = "Ordinary Income/Expense";
            ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, col).Style.Font.FontColor = XLColor.Black;
            ws.Cell(row, col).Style.Font.Bold = true;
            ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            col++;
            foreach (var header in _tableHeader)
            {

                //WriteCell(row, col, header);
                ws.Cell(row, col).Value = header;
                ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(row, col).Style.Font.FontColor = XLColor.Black;
                ws.Cell(row, col).Style.Font.Bold = true;
                ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                col++;
            }

            ws.Cell(row, col).Value = "Total";
            ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, col).Style.Font.FontColor = XLColor.Black;
            ws.Cell(row, col).Style.Font.Bold = true;
            ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            col++;
            row++;

            ExportProfitLossRecursive(dataList, ref row, 0, _tableHeader.ToList());
        }


        public void ExportProfitLossRecursive(ICollection<T> items, ref int row, int level = 0, List<string> relevantProps = null)
        {
            foreach (var item in items)
            {
                int col = 1;

                var type = typeof(T);
                var accountNameProp = type.GetProperty("AccountName");
                var selfBalanceProp = type.GetProperty("SelfBalance");
                var balanceProp = type.GetProperty("Balance");
                var childrenProp = type.GetProperty("Children");
                var parentAccountProp = type.GetProperty("ParentAccount");
                var monthBalancesProp = type.GetProperty("MonthBalances");

                string accountName = new string(' ', level * 4) + (accountNameProp?.GetValue(item)?.ToString() ?? "");
                decimal selfBalance = (decimal?)selfBalanceProp?.GetValue(item) ?? 0;

                decimal totalBalance = CalculateTotalBalance(item);

                string parentAccount = parentAccountProp?.GetValue(item)?.ToString();
                var monthBalances = monthBalancesProp?.GetValue(item) as Dictionary<string, decimal>;

                // First column: Account Name
                WriteCell(row, col++, accountName);



                // Add team-wise balance columns
                if (relevantProps != null)
                {
                    foreach (var team in relevantProps)
                    {
                        if (team == "Ordinary Income/Expense" || team == "Total") continue; // skip if included in relevantProps
                        decimal monthBalance = monthBalances != null && monthBalances.TryGetValue(team, out var value) ? value : 0;
                        WriteCell(row, col++, monthBalance);
                    }
                }

                // Total Balance (all teams)
                WriteCell(row, col++, totalBalance);

                // Highlight Total or GrossNet rows
                if (parentAccount == "Total" || parentAccount == "GrossNet")
                {
                    var lastCol = 1 + 1 + (relevantProps?.Count ?? 0); // 1 for name, 1 for balance
                    var highlightRange = ws.Range(row, 1, row, lastCol);
                    highlightRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    highlightRange.Style.Font.Bold = true;
                    highlightRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Align text properly
                    ws.Cell(row, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left); // Account Name
                    for (int i = 2; i <= lastCol; i++)
                    {
                        ws.Cell(row, i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                }

                row++;

                // Recursive children
                var children = childrenProp?.GetValue(item) as IEnumerable<T>;
                if (children != null && children.Any())
                {
                    ExportProfitLossRecursive(children.ToList(), ref row, level + 1, relevantProps);
                }
            }
        }

        public decimal CalculateTotalBalance(T item)
        {
            var type = typeof(T);
            var selfBalanceProp = type.GetProperty("SelfBalance");
            var childrenProp = type.GetProperty("Children");

            decimal self = (decimal?)selfBalanceProp?.GetValue(item) ?? 0;
            decimal childSum = 0;

            var children = childrenProp?.GetValue(item) as IEnumerable<T>;
            if (children != null && children.Any())
            {
                foreach (var child in children)
                {
                    childSum += CalculateTotalBalance(child);
                }
            }

            return self + childSum;
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
    }
}
