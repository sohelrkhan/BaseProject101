namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardTableExcelService<T, F> : BaseExcelService<T, F> where F : IFilterItem
    {
        public bool _needcalculation { get; set; }
        public string _groupedBy { get; set; }

        private readonly bool _isGrouped;


        public StandardTableExcelService(bool needCalculation = false, bool isGrouped = false, string groupedBy = null)
        {
            _needcalculation = needCalculation;
            _isGrouped = isGrouped;
            _groupedBy = groupedBy;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (_isGrouped)
            {
                props = props
                        .Where(p =>
                            p.Name != "Code" &&
                            (p.GetCustomAttribute<ExcludeFromTableAttribute>() == null))
                        .ToArray();
            }

            int col = 1;

            var headerRow = row;

            foreach (var prop in props)
            {
                if (prop.Name != "Code" && prop.Name != _groupedBy && prop.GetCustomAttribute<ExcludeFromTableAttribute>()==null)
                {
                    // Try to get the display header attribute
                    var displayAttr = prop.GetCustomAttribute<DisplayHeaderAttribute>();
                    string label;

                    if (displayAttr != null)
                    {
                        label = displayAttr.HeaderText;
                    }
                    else
                    {
                        label = prop.Name?.ToSentenceCase(); 
                    }

                    ws.Cell(row, col).Value = label;
                    ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(row, col).Style.Font.FontColor = XLColor.Black;
                    ws.Cell(row, col).Style.Font.Bold = true;
                    ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    col++;
                }
            }

            row++; // Move to data rows
            int startDataRow = row; // Record start of data rows

            // Data Rows
            foreach (var item in dataList)
            {
                col = 1;
                foreach (var prop in props)
                {
                    if (prop.Name == "Code" || prop.Name == _groupedBy || prop.GetCustomAttribute<ExcludeFromTableAttribute>() != null) continue;

                    if (prop.Name.EndsWith("Photo", StringComparison.OrdinalIgnoreCase))
                    {
                        var imgBytes = prop.GetValue(item) as byte[];

                        if (imgBytes != null && imgBytes.Length > 0)
                        {
                            using var ms = new MemoryStream(imgBytes);

                            // Set column width and row height slightly larger for the photo
                            ws.Column(col).Width = 20;
                            ws.Row(row).Height = 40;

                            using var img = System.Drawing.Image.FromStream(new MemoryStream(imgBytes));
                            var imgWidth = img.Width;
                            var imgHeight = img.Height;

                            double cellWidthPx = ws.Column(col).Width * 7.5;
                            double cellHeightPx = ws.Row(row).Height * 1.33;

                            double scaleX = (cellWidthPx * 0.85) / imgWidth;
                            double scaleY = (cellHeightPx * 0.85) / imgHeight;
                            double scale = Math.Min(scaleX, scaleY);

                            double finalWidth = imgWidth * scale;
                            double finalHeight = imgHeight * scale;

                            int offsetX = (int)((cellWidthPx - finalWidth) / 2);
                            int offsetY = (int)((cellHeightPx - finalHeight) / 2);

                            ws.AddPicture(ms)
                                .MoveTo(ws.Cell(row, col), offsetX, offsetY)
                                .Scale((float)scale);
                        }
                        else
                        {
                            // Fallback for missing image
                            ws.Cell(row, col).Value = "No Photo";
                            ws.Cell(row, col).Style.Font.Italic = true;
                            ws.Cell(row, col).Style.Font.FontSize = 8;
                            ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(row, col).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    else
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
                        if(value is not string && value is not string DateTime)
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
                    }

                    col++;
                }
                row++;
            }

            // Footer Total Row
            if (_needcalculation)
            {
                int endDataRow = row - 1; // last row of data
                int totalRow = row;       // total row will be written here
                col = 1;
                bool totalLabelWritten = false;

                foreach (var prop in props)
                {
                    if (prop.Name == "Code" || prop.GetCustomAttribute<ExcludeFromTableAttribute>() != null) continue;

                    var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    bool skipCalculation = prop.GetCustomAttribute<SkipCalculationAttribute>() != null || prop.GetCustomAttribute<ExcludeFromTableAttribute>() != null;


                    if (!totalLabelWritten)
                    {
                        ws.Cell(totalRow, col).Value = "Total";
                        ws.Cell(totalRow, col).Style.Font.Bold = true;
                        ws.Cell(totalRow, col).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        ws.Cell(totalRow, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(totalRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        totalLabelWritten = true;
                    }
                    else if (skipCalculation is true)
                    {
                        ws.Cell(totalRow, col).Value = "";
                        ws.Cell(totalRow, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                        ws.Cell(totalRow, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(totalRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }
                    else if (propType == typeof(decimal) || propType == typeof(double) ||
                             propType == typeof(int) || propType == typeof(float) || propType == typeof(long))
                    {
                        string columnLetter = GetExcelColumnLetter(col);
                        string formula = $"SUM({columnLetter}{startDataRow}:{columnLetter}{endDataRow})";
                        var cell = ws.Cell(totalRow, col);
                        cell.FormulaA1 = formula;
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // Set number format based on data type
                        if (propType == typeof(decimal) || propType == typeof(double) || propType == typeof(float))
                        {
                            cell.Style.NumberFormat.Format = "#,##0.00"; // 2 decimal places
                        }
                        else if (propType == typeof(int) || propType == typeof(long))
                        {
                            cell.Style.NumberFormat.Format = "#,##0"; // No decimal
                        }
                    }
                    else
                    {
                        ws.Cell(totalRow, col).Value = "";
                        ws.Cell(totalRow, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                        ws.Cell(totalRow, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(totalRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    col++;
                }

                row += 2;
            }
        }

        private string GetExcelColumnLetter(int colIndex)
        {
            string colLetter = "";
            while (colIndex > 0)
            {
                colIndex--;
                colLetter = (char)('A' + (colIndex % 26)) + colLetter;
                colIndex /= 26;
            }
            return colLetter;
        }
    }
}
