namespace SadaqaAccounting.Shared.ReportHelpers
{
    public abstract class BaseExcelService<T, F> where F : IFilterItem
    {
        public XLWorkbook workbook;
        public IXLWorksheet ws;
        public int row = 3;

        public PropertyInfo[] props { get; set; }

        public byte[] ExportToExcel(string title, string companyName, string companyAddress, string email, string website, string phoneNumber, string createdByName,
            ICollection<F> filters, ICollection<T> dataList, byte[]? logoImage = null, byte[]? qrCodeImage = null, bool isGrouped = false)
        {
            props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            props = props
                    .Where(p =>
                        p.Name != "Code" &&
                        (p.GetCustomAttribute<ExcludeFromTableAttribute>() == null))
                    .ToArray();

            workbook = new XLWorkbook();
            if (string.IsNullOrEmpty(title))
            {
                ws = workbook.Worksheets.Add("Tab 1");
            }
            else
            {
                ws = workbook.Worksheets.Add(title);
            }

            AddLogos(logoImage, qrCodeImage);
            AddCompanyInfo(companyName, companyAddress);
            AddCreator(createdByName);
            AddFilters(filters);

            if (!string.IsNullOrEmpty(title))
            {
                AddTitle(title);
                row += 2;
            }

            ComposeTable(dataList);

            //AddFooter(website, email, phoneNumber);

            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void AddLogos(byte[] logoImage, byte[] qrcodeImage)
        {
            if (logoImage != null && logoImage.Length > 0)
            {
                scaleImageAndAddToRow(logoImage, 1);
            }
            else
            {
                ws.Cell(2, 1).Value = "No Logo";
                ws.Cell(2, 1).Style.Font.Italic = true;
                ws.Cell(2, 1).Style.Font.FontSize = 20;
                ws.Cell(2, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(2, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Row(2).Height = 30;
                ws.Column(1).Width = 20;
            }

            if (qrcodeImage != null && qrcodeImage.Length > 0)
            {
                scaleImageAndAddToRow(qrcodeImage, props.Length);
            }
        }

        private void scaleImageAndAddToRow(byte[] image, int col)
        {
            using var ms = new MemoryStream(image);
            var customRow = 2;
            // Set column width and row height slightly larger for the photo
            ws.Column(col).Width = 20;
            ws.Row(customRow).Height = 100;

            // Get the dimensions of the image
            using var img = System.Drawing.Image.FromStream(new MemoryStream(image));
            var imgWidth = img.Width;
            var imgHeight = img.Height;

            // Calculate available cell space in pixels
            double cellWidthPx = ws.Column(col).Width * 7.5;   // approx
            double cellHeightPx = ws.Row(customRow).Height * 1.33;   // approx

            // Scale image to 80–90% of cell size for visibility
            double scaleX = (cellWidthPx * 0.85) / imgWidth;
            double scaleY = (cellHeightPx * 0.85) / imgHeight;
            double scale = Math.Min(scaleX, scaleY);

            // Recalculate final image size
            double finalWidth = imgWidth * scale;
            double finalHeight = imgHeight * scale;

            // Calculate offsets to center the image
            int offsetX = (int)((cellWidthPx - finalWidth) / 2);
            int offsetY = (int)((cellHeightPx - finalHeight) / 2);
            ws.AddPicture(ms)
                            .MoveTo(ws.Cell(customRow, col), offsetX, offsetY)
                            .Scale((float)scale);
        }

        private void AddCompanyInfo(string companyName, string companyAddress)
        {
            ws.Cell(row, 1).Value = companyName;
            ws.Cell(row, 1).Style.Font.SetBold().Font.FontSize = 15;
            row++;
            ws.Cell(row, 1).Value = companyAddress;
            ws.Range(row, 1, row, 5).Style.Font.SetBold().Font.FontSize = 10;
        }

        private void AddFilters(ICollection<F> filters)
        {
            foreach (var filter in filters)
            {
                var key = filter.GetType().GetProperty("Key")?.GetValue(filter)?.ToString();
                var value = filter.GetType().GetProperty("Value")?.GetValue(filter)?.ToString();

                if (!string.IsNullOrWhiteSpace(key) && key != "Company" && !string.IsNullOrWhiteSpace(value))
                {
                    ws.Cell(row, 1).Value = $"{key} : {value}";
                    ws.Cell(row, 1).Style.Font.FontColor = XLColor.Blue;
                    row++;
                }
            }

            row++;
        }

        private void AddCreator(string createdBy)
        {
            var lastColumn = props.Length;
            ws.Cell(row, lastColumn).Value = $"Created Date : {DateTime.Now:dd MMM yyyy}";
            row++;
            ws.Cell(row, lastColumn).Value = $"Created By : {createdBy}";
        }

        private void AddTitle(string title)
        {
            var col = props.Length > 1 ? props.Length > 22 ? 22 : props.Length / 2 : 1;
            ws.Cell(row, col).Value = title;
            ws.Cell(row, col).Style.Font.Bold = true;
            ws.Cell(row, col).Style.Font.FontSize = 15;
            ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        public abstract void ComposeTable(ICollection<T> dataList);
    }
}