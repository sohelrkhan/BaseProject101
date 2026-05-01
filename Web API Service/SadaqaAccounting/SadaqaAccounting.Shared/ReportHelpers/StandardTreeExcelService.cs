namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardTreeExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        public bool _needcalculation { get; set; }
        public string _groupedBy { get; set; }

        private readonly bool _isGrouped;


        public StandardTreeExcelService(bool needCalculation = false, bool isGrouped = false, string groupedBy = null)
        {
            _needcalculation = needCalculation;
            _isGrouped = isGrouped;
            _groupedBy = groupedBy;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int col = 1;

            var headerRow = row;

            // Header Row
            foreach (var prop in props)
            {
                var label = prop.Name?.ToSentenceCase();

                ws.Cell(row, col).Value = label;
                ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(row, col).Style.Font.FontColor = XLColor.Black;
                ws.Cell(row, col).Style.Font.Bold = true;
                ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                col++;
            }

            row++; // Move to data rows
            int startDataRow = row; // Record start of data rows

            // Data Rows
            foreach (var item in dataList)
            {
                col = 1;
                foreach (var prop in props)
                {
                    var value = prop.GetValue(item);


                    if (value is DateTime dt)
                        ws.Cell(row, col).Value = dt;
                    else if (value is int i)
                        ws.Cell(row, col).Value = i;
                    else if (value is decimal d)
                    {
                        ws.Cell(row, col).Value = Math.Round(d, 2);
                        ws.Cell(row, col).Style.NumberFormat.Format = "#,##0.00";
                    }
                    else if (value is double db)
                    {
                        ws.Cell(row, col).Value = Math.Round(db, 2);
                        ws.Cell(row, col).Style.NumberFormat.Format = "#,##0.00";
                    }
                    else if (value is float f)
                    {
                        ws.Cell(row, col).Value = Math.Round(f, 2);
                        ws.Cell(row, col).Style.NumberFormat.Format = "#,##0.00";
                    }
                    else if (value is long l)
                        ws.Cell(row, col).Value = l;
                    else if (value != null)
                        ws.Cell(row, col).Value = value.ToString();
                    else
                        ws.Cell(row, col).Value = "";



                    ws.Cell(row, col).Style.Font.FontSize = 9;
                    if (value is not string && value is not string DateTime)
                    {
                        ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(headerRow, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    else
                    {
                        ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    }
                    ws.Cell(row, col).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    col++;
                }
                row++;
            }
            
        }
    }
}
