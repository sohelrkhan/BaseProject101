namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class ExportToExcelService <T, F> where F : IFilterItem
    {
        public byte[] ExportToExcel(string title , string filePath, string companyName, string companyAddress, string email, string website, string phoneNumber, string createdByName,
            ICollection<F> filters, ICollection<T> dataList, byte[]? logoImage = null, byte[]? qrCodeImage = null)
        {
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(title);

            int row = 6;

            if (logoImage != null)
            {
                using var ms = new MemoryStream(logoImage);
                var img = ws.AddPicture(ms).MoveTo(ws.Cell("B1")).Scale(0.4);
            }

            if (qrCodeImage != null)
            {
                using var ms = new MemoryStream(qrCodeImage);
                var img = ws.AddPicture(ms).MoveTo(ws.Cell("G1")).Scale(0.2);
            }

            // Company Info
            ws.Cell(row, 2).Value = companyName;
            ws.Cell(row, 2).Style.Font.SetBold().Font.FontSize = 15;
            row++;

            ws.Cell(row , 2).Value = companyAddress;
            ws.Range(row, 2, row, 5).Style.Font.SetBold().Font.FontSize = 10;
            row ++;

            // Filters
            foreach (var filter in filters)
            {
                if (filter != null && filter.GetType().GetProperty("Value")?.GetValue(filter) != null)
                {
                    var key = filter.GetType().GetProperty("Key")?.GetValue(filter)?.ToString();
                    var value = filter.GetType().GetProperty("Value")?.GetValue(filter)?.ToString();

                    if (key != "Company")
                    {
                        ws.Cell(row, 1).Value = $"{key}:";
                        ws.Cell(row, 2).Value = value;
                        row++;
                    }
                }
            }

            row ++;

            // Title and Created Info
            ws.Cell(row, 4).Value = title;
            ws.Cell(row, 4).Style.Font.Bold = true;
            ws.Cell(row, 4).Style.Font.FontSize = 15;
            ws.Cell(row, 7).Value = $"Created Date : {DateTime.Now.ToString("dd MMM yyyy")}";
            row++;

            ws.Cell(row, 7).Value = $"Created By : {createdByName}";
            row ++;

            // Table Header
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int col = 1;

            foreach (var prop in props)
            {
                if (prop.Name != "Code")
                {
                    ws.Cell(row, col).Value = prop.Name.ToSentenceCase();
                    ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(row, col).Style.Font.Bold = true;
                    ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    col++;
                }
            }

            row++;

            // Table Data
            foreach (var item in dataList)
            {
                col = 1;
                foreach (var prop in props)
                {
                    if (prop.Name != "Code")
                    {
                        var value = prop.GetValue(item)?.ToString();
                        ws.Cell(row, col).Value = value;
                        ws.Cell(row, col).Style.Font.FontSize = 9;
                        ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        col++;
                    }
                }

                row++;
            }

            row += 2;

            // Footer Section
            ws.Cell(row, 2).Value = "This is a system-generated report and does not require a signature. To verify its authenticity, please scan the QR code in the header.";
            ws.Range(row, 2, row, 8).Merge().Style.Font.Italic = true;
            var range = ws.Range(row, 2, row, 8); 
            range.Merge(); 
            range.Style.Font.FontColor = XLColor.Red; 
            row++;

            ws.Cell(row, 2).Value = $"Website: {website}     Email: {email}     Mobile: {phoneNumber}";
            ws.Range(row, 2, row, 6).Merge().Style.Font.FontSize = 8;
            ws.Range(row, 2, row, 6).Merge().Style
           .Font.Italic = true;

            // Auto-fit everything
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}