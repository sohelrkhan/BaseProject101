namespace SadaqaAccounting.Application.Configurations.DownloadPDFs
{
    //public static class DownloadPDFProvider
    //{
    //    public static string ExportReportToPdf<T>(this IWebHostEnvironment _webHostEnvironment, List<T> data, List<string> filters) where T : class
    //    {
    //        var type = typeof(T);

    //        var propertyNames = type.GetProperties().Select(s => string.Join(" ", Regex.Split(s.Name, @"(?<!^)(?=[A-Z])")));
    //        int count = propertyNames.Count();

    //        var path = Path.Combine(_webHostEnvironment.WebRootPath, "ExportedFiles", "PF");

    //        if (!Directory.Exists(path))
    //        {
    //            Directory.CreateDirectory(path);
    //        }

    //        path = Path.Combine(path, type.Name + ".pdf");

    //        using (FileStream stream = new FileStream(path, FileMode.Create))
    //        {
    //            Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
    //            PdfWriter.GetInstance(doc, stream);
    //            doc.Open();

    //            //Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
    //            //Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

    //            //BaseColor titleColor = new BaseColor(31, 73, 125); // Navy Blue
    //            //Font companyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, titleColor);
    //            //Font filterFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10);

    //            //for (int i = 0; i < filters.Count; i++)
    //            //{
    //            //    Font fontToUse = boldFont;

    //            //    if (i == 0)
    //            //    {
    //            //        fontToUse = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, BaseColor.BLUE);
    //            //    }

    //            //    // No SpacingAfter at all
    //            //    Paragraph p = new Paragraph(filters[i], fontToUse);
    //            //    doc.Add(p);
    //            //}
    //            //--------------------------------------------------------------------------------------------------------
    //            // Define fonts and color as before
    //            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
    //            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
    //            BaseColor titleColor = new BaseColor(31, 73, 125); // Navy Blue
    //            Font companyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, titleColor);
    //            Font filterFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10);
    //            Font downloadByFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8);
    //            // Extract values from your filter list
    //            string companyName = filters.FirstOrDefault() ?? "";
    //            string downloadBy = filters.FirstOrDefault(x => x.StartsWith("Download By:"))!.Split(':').Last().Trim();
    //            var raw = filters.FirstOrDefault(x => x.StartsWith("Download Date:"));
    //            var downloadDateValue = raw?.Split(':', 2)[1].Trim(); // get "19 May 2025 03:27 PM"
    //            string downloadDate = downloadDateValue!;

    //            // Remove Download info from the filter list so it's not printed twice
    //            var filteredFilters = filters
    //                .Skip(1) // skip company name
    //                .Where(x => !x.StartsWith("Download By:") && !x.StartsWith("Download Date:"))
    //                .ToList();

    //            // Create header table with company on left, download info on right
    //            PdfPTable headerTable = new PdfPTable(2);
    //            headerTable.WidthPercentage = 100;
    //            headerTable.SetWidths(new float[] { 70f, 30f }); // adjust width ratio

    //            PdfPCell leftCell = new PdfPCell(new Phrase(companyName, companyFont));
    //            leftCell.Border = Rectangle.NO_BORDER;
    //            leftCell.HorizontalAlignment = Element.ALIGN_LEFT;

    //            PdfPCell rightCell = new PdfPCell(new Phrase($"{downloadBy} | {downloadDate}", downloadByFont));
    //            rightCell.Border = Rectangle.NO_BORDER;
    //            rightCell.HorizontalAlignment = Element.ALIGN_RIGHT;

    //            headerTable.AddCell(leftCell);
    //            headerTable.AddCell(rightCell);

    //            // Add to document
    //            doc.Add(headerTable);

    //            // Add filters below
    //            foreach (var filter in filteredFilters)
    //            {
    //                Paragraph p = new Paragraph(filter, filterFont);
    //                doc.Add(p);
    //            }

    //            //-----------------------------------------------------------------------------

    //            doc.Add(new Paragraph(" ")); // Space before table

    //            // DATA TABLE
    //            PdfPTable table = new PdfPTable(typeof(T).GetProperties().Length)
    //            {
    //                WidthPercentage = 100
    //            };

    //            // Headers
    //            foreach (var prop in typeof(T).GetProperties())
    //            {
    //                string header = string.Join(" ", Regex.Split(prop.Name, @"(?<!^)(?=[A-Z])"));
    //                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

    //                var cell = new PdfPCell(new Phrase(header, boldFont))
    //                {
    //                    BackgroundColor = BaseColor.LIGHT_GRAY,
    //                    HorizontalAlignment = (propType == typeof(decimal) || propType == typeof(double) || propType == typeof(float) || propType == typeof(int))
    //                        ? Element.ALIGN_RIGHT
    //                        : Element.ALIGN_LEFT
    //                };

    //                table.AddCell(cell);
    //            }


    //            // Data Rows
    //            foreach (var row in data)
    //            {
    //                foreach (var prop in typeof(T).GetProperties())
    //                {
    //                    var value = prop.GetValue(row);
    //                    var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

    //                    string cellText = "";
    //                    if (value != null)
    //                    {
    //                        if (propType == typeof(decimal) || propType == typeof(double) || propType == typeof(float) || propType == typeof(int))
    //                        {
    //                            cellText = Convert.ToDecimal(value).ToString("N2");
    //                        }
    //                        else
    //                        {
    //                            cellText = value.ToString()!;
    //                        }
    //                    }

    //                    PdfPCell valueCell = new PdfPCell(new Phrase(cellText, normalFont))
    //                    {
    //                        HorizontalAlignment = (propType == typeof(decimal) || propType == typeof(double) || propType == typeof(float) || propType == typeof(int))
    //                            ? Element.ALIGN_RIGHT
    //                            : Element.ALIGN_LEFT
    //                    };

    //                    table.AddCell(valueCell);
    //                }
    //            }

    //            // Get model type from T
    //            var modelType = typeof(T);
    //            var props = modelType.GetProperties();

    //            Font totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
    //            PdfPCell totalLabel = new PdfPCell(new Phrase("Total", totalFont))
    //            {
    //                Colspan = 0, // We'll count how many non-numeric columns to span
    //                HorizontalAlignment = Element.ALIGN_RIGHT,
    //                BackgroundColor = BaseColor.LIGHT_GRAY
    //            };

    //            List<(int Index, decimal Total)> numericTotals = new();

    //            for (int i = 0; i < props.Length; i++)
    //            {
    //                var prop = props[i];
    //                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

    //                if (propType == typeof(decimal) || propType == typeof(double) || propType == typeof(float) || propType == typeof(int))
    //                {
    //                    decimal total = data.Sum(x =>
    //                    {
    //                        var val = prop.GetValue(x);
    //                        return val != null ? Convert.ToDecimal(val) : 0;
    //                    });

    //                    numericTotals.Add((i, total));
    //                }
    //                else
    //                {
    //                    totalLabel.Colspan++;
    //                }
    //            }
    //            if (numericTotals.Count > 0)
    //            {
    //                // Add the label spanning all non-numeric columns
    //                table.AddCell(totalLabel);
    //            }
                

    //            // Now fill in blank cells between non-numeric and first numeric if needed
    //            int filled = totalLabel.Colspan;
                
    //            if(numericTotals.Any())
    //            {
    //                for (int i = filled; i < numericTotals[0].Index; i++)
    //                {
    //                    table.AddCell(""); // padding if needed
    //                }
    //            }

    //            // Add each total to its correct numeric column
    //            foreach (var (Index, Total) in numericTotals)
    //            {
    //                var cell = new PdfPCell(new Phrase(Total.ToString("N2"), totalFont))
    //                {
    //                    HorizontalAlignment = Element.ALIGN_RIGHT,
    //                    BackgroundColor = BaseColor.LIGHT_GRAY
    //                };
    //                table.AddCell(cell);
    //            }


    //            doc.Add(table);
    //            doc.Close();
    //        }

    //        return path;
    //    }
    //}
}
