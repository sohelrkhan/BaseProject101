using Color = DocumentFormat.OpenXml.Spreadsheet.Color;
using FontSize = DocumentFormat.OpenXml.Spreadsheet.FontSize;

namespace SadaqaAccounting.Application.Configurations.DownloadExcels
{
    public static class DownloadExcelProvider
    {
        public static string CreateSpreadsheetWorkbook<T>(this IWebHostEnvironment _webHostEnvironment, List<T> data, List<string> filters, 
            string folderName = "PF", bool isCalculateTotalValue = true, string reportTitle = "Report Title") where T : class
        {
            var type = typeof(T);
            var propertyNames = type.GetProperties().Select(s => string.Join(" ", Regex.Split(s.Name, @"(?<!^)(?=[A-Z])")));
            int count = propertyNames.Count();

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "ExportedFiles", folderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, type.Name + ".xlsx");

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();
                WorkbookStylesPart stylesPart = AddStyles(workbookpart);

                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = type.Name
                };
                sheets.Append(sheet);

                SharedStringTablePart shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
                string[] alphabats = new string[count];

                for (int i = 0; i < count; i++)
                    alphabats[i] = GenerateColumnIndex(i);

                uint reportTitleRow = 1;
                int titleIndex = InsertSharedStringItem(reportTitle, shareStringPart);
                Cell titleCell = InsertCellInWorksheet("A", reportTitleRow, worksheetPart, 0);
                titleCell.CellValue = new CellValue(titleIndex.ToString());
                titleCell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                titleCell.StyleIndex = 2U;

                // Merge A1 to last header column (e.g., E1)
                MergeCells mergeCells = new MergeCells();
                string endCell = GenerateColumnIndex(count - 1) + "1";
                mergeCells.Append(new MergeCell() { Reference = new StringValue($"A1:{endCell}") });

                if (!worksheetPart.Worksheet.Elements<MergeCells>().Any())
                {
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                }

                uint filterRowIndex = 2;

                for (int i = 0; i < filters.Count; i++)
                {
                    int index = InsertSharedStringItem(filters[i], shareStringPart);
                    Cell cell = InsertCellInWorksheet("A", filterRowIndex, worksheetPart, 0);
                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    cell.StyleIndex = (i == 0) ? 2U : 1U;
                    filterRowIndex++;
                }

                uint headerRowIndex = filterRowIndex;
                var properties = type.GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    string columnName = properties[i].Name;
                    int index = InsertSharedStringItem(columnName, shareStringPart);
                    Cell cell = InsertCellInWorksheet(alphabats[i], headerRowIndex, worksheetPart, i);
                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    cell.StyleIndex = 4U;
                }

                uint row = headerRowIndex + 1;

                foreach (var item in data)
                {
                    for (int i = 0; i < count; i++)
                    {
                        string value = type.GetProperties()[i]?.GetValue(item)?.ToString() ?? "";
                        Cell cell = InsertCellInWorksheet(alphabats[i], row, worksheetPart, i);

                        if (decimal.TryParse(value, out var number))
                        {
                            cell.CellValue = new CellValue(number.ToString(CultureInfo.InvariantCulture));
                            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        }
                        else
                        {
                            int index = InsertSharedStringItem(value, shareStringPart);
                            cell.CellValue = new CellValue(index.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                        }
                    }
                    row++;
                }

                if (isCalculateTotalValue)
                {
                    uint totalRowIndex = row;
                    var numericProps = properties
                        .Select((p, i) => new { Property = p, Index = i })
                        .Where(x => x.Property.PropertyType == typeof(int) ||
                                    x.Property.PropertyType == typeof(decimal) ||
                                    x.Property.PropertyType == typeof(double) ||
                                    x.Property.PropertyType == typeof(float))
                        .ToList();

                    if (numericProps.Count > 0)
                    {
                        Cell totalLabelCell = InsertCellInWorksheet("A", totalRowIndex, worksheetPart, 0);
                        totalLabelCell.CellValue = new CellValue("Total");
                        totalLabelCell.DataType = new EnumValue<CellValues>(CellValues.String);
                        totalLabelCell.StyleIndex = 4U;
                    }

                    foreach (var np in numericProps)
                    {
                        string colLetter = GenerateColumnIndex(np.Index);
                        Cell totalCell = InsertCellInWorksheet(colLetter, totalRowIndex, worksheetPart, np.Index);
                        totalCell.CellFormula = new CellFormula($"SUM({colLetter}{headerRowIndex + 1}:{colLetter}{row - 1})");
                        totalCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        totalCell.StyleIndex = 4U;
                    }

                    spreadsheetDocument.WorkbookPart.Workbook.CalculationProperties = new CalculationProperties()
                    {
                        FullCalculationOnLoad = true
                    };
                }

                worksheetPart.Worksheet.Save();
                workbookpart.Workbook.Save();
            }

            return path;
        }

        private static WorkbookStylesPart AddStyles(WorkbookPart workbookPart)
        {
            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = new Stylesheet();

            // 1. Fonts
            DocumentFormat.OpenXml.Drawing.Fonts fonts = new DocumentFormat.OpenXml.Drawing.Fonts(
                new Font(),                             // Index 0 - default
                new Font(new Bold()),                   // Index 1 - bold
                new Font(new FontSize() { Val = 24 },   // Index 2 - title font
                          new Color() { Rgb = "2E75B6" }),
                new Font(new Bold(), new Color() { Rgb = "FFFFFF" }) // Index 3 - bold white
            );

            // 2. Fills (background colors)
            Fills fills = new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }),               // 0 - default
                new Fill(new PatternFill(new ForegroundColor { Rgb = "D9D9D9" })                // 1 - gray
                { PatternType = PatternValues.Solid }),
                new Fill(new PatternFill(new ForegroundColor { Rgb = "2E75B6" })                // 2 - blue
                { PatternType = PatternValues.Solid })
            );

            // 3. Borders
            Borders borders = new Borders(new Border());

            // 4. CellFormats
            CellFormats cellFormats = new CellFormats(
                new CellFormat(),                                 // 0 - default
                new CellFormat() { FontId = 1, ApplyFont = true },// 1 - bold
                new CellFormat() { FontId = 2, ApplyFont = true },// 2 - title
                new CellFormat() { FontId = 1, FillId = 1, ApplyFont = true, ApplyFill = true }, // 3 - gray bold
                new CellFormat() { FontId = 3, FillId = 2, ApplyFont = true, ApplyFill = true }  // 4 - blue background, white bold
            );

            stylesPart.Stylesheet.Append(fonts);
            stylesPart.Stylesheet.Append(fills);
            stylesPart.Stylesheet.Append(borders);
            stylesPart.Stylesheet.Append(cellFormats);
            stylesPart.Stylesheet.Save();

            return stylesPart;
        }
        public static string GenerateColumnIndex(int num)
        {
            string str = "";
            char achar;
            int mod;

            do
            {
                mod = (num % 26) + 65;
                num = (int)(num / 26);
                achar = (char)mod;
                str = achar + str;
                num--;
            } while (num >= 0);

            return str;
        }
        // Helper method to handle SharedStringTable
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }

            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart, int index)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>()!;
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex! == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex! == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference!.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference!.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                if (index < 26)
                {
                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        var val = string.Compare(cell.CellReference!.Value, cellReference, true);
                        if (val > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (Cell cell in row.Elements<Cell>().Skip(26))
                    {
                        var val = string.Compare(cell.CellReference!.Value, cellReference, true);
                        if (val > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}