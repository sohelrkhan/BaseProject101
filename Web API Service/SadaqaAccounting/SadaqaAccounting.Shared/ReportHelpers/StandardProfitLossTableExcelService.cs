namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardProfitLossTableExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        public StandardProfitLossTableExcelService()
        {

        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            //int row = 1;

            //// Header
            //WriteCell(row, 1, "Account Name");
            //WriteCell(row, 2, "Self Balance");
            //WriteCell(row, 3, "Total Balance");
            //row++;

            ExportProfitLossRecursive(dataList, ref row);
        }


        public void ExportProfitLossRecursive(ICollection<T> items, ref int row, int level = 0)
        {
            foreach (var item in items)
            {
                int col = 1;

                // Get properties by reflection
                var type = typeof(T);
                var accountNameProp = type.GetProperty("AccountName");
                var selfBalanceProp = type.GetProperty("SelfBalance");
                var childrenProp = type.GetProperty("Children");
                var parentAccountProp = type.GetProperty("ParentAccount");

                string accountName = new string(' ', level * 4) + (accountNameProp?.GetValue(item)?.ToString() ?? "");
                decimal selfBalance = (decimal?)selfBalanceProp?.GetValue(item) ?? 0;
                decimal totalBalance = CalculateTotalBalance(item);
                string parentAccount = parentAccountProp?.GetValue(item)?.ToString();

                WriteCell(row, col++, accountName);
                WriteCell(row, col++, totalBalance);

                // ✅ Apply background color if ParentAccount == "Total" or "GrossNet"
                if (parentAccount == "Total" || parentAccount == "GrossNet")
                {
                    var highlightRange = ws.Range(row, 1, row, 2); // Adjust to your total number of columns
                    highlightRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    highlightRange.Style.Font.Bold = true;
                    highlightRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // 👈 Only column 1 (Account Name) left-aligned
                    ws.Cell(row, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    // 👉 Only column 2 (Balance) right-aligned
                    ws.Cell(row, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                }

                row++;

                var children = childrenProp?.GetValue(item) as IEnumerable<T>;
                if (children != null && children.Any())
                {
                    ExportProfitLossRecursive(children.ToList(), ref row, level + 1);
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
