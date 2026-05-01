namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardTablePdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly bool _isGrouped;
        public bool _needcalculation { get; set; }

        public StandardTablePdfService(bool needCalculation = false, bool isGrouped = false)
        {
            _needcalculation = needCalculation;
            _isGrouped = isGrouped;
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string groupByPropertyName = null)
        {

            var allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var properties = allProperties
                .Where(p =>
                    p.Name != "Code" &&
                    p.Name != groupByPropertyName &&
                    p.GetCustomAttribute<ExcludeFromTableAttribute>() == null)
                .ToList();

            var numericProperties = new List<PropertyInfo>();
            var numericSums = new Dictionary<string, decimal>();

            foreach (var prop in properties)
            {
                if (Attribute.IsDefined(prop, typeof(SkipCalculationAttribute)))
                {
                    continue;
                }

                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (IsNumericType(propType))
                {
                    numericProperties.Add(prop);
                    numericSums[prop.Name] = 0;
                }
            }


            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "Code" && prop.Name != groupByPropertyName)
                            columns.RelativeColumn();
                    }
                });

                table.Header(header =>
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "Code" && prop.Name != groupByPropertyName)
                        {
                            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            bool isNumeric = IsNumericType(type);

                            var displayAttr = prop.GetCustomAttribute<DisplayHeaderAttribute>();
                            var headerText = displayAttr != null ? displayAttr.HeaderText : prop.Name.ToSentenceCase();

                            header.Cell()
                                  .Element(isNumeric ? CellStyleHeaderRight : CellStyleHeader)
                                  .Text(headerText)
                                  .SemiBold()
                                  .FontSize(9);
                        }
                    }
                });


                // Data Rows
                foreach (var item in dataList)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name == "Code" || prop.Name == groupByPropertyName)
                            continue;

                        if (prop.Name.EndsWith("Photo", StringComparison.OrdinalIgnoreCase))
                        {
                            var photoValue = prop.GetValue(item);
                            if (photoValue is byte[] photoBytes && photoBytes.Length > 0)
                            {
                                table.Cell().Element(CellStyle).Height(50).Image(photoBytes, ImageScaling.FitArea);
                            }
                            else
                            {
                                table.Cell().Element(CellStyle).Height(50).AlignCenter().AlignMiddle().Text("No Image").FontSize(6).Italic();
                            }
                        }
                        else
                        {
                            var value = prop.GetValue(item);
                            string displayValue = "";
                            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                            if (value != null)
                            {
                                if (IsNumericType(type))
                                {
                                    try
                                    {
                                        decimal val = Convert.ToDecimal(value);
                                        displayValue = IsFloatingPointType(type)
                                            ? val.ToString("N2", CultureInfo.InvariantCulture)
                                            : val.ToString("N2", CultureInfo.InvariantCulture);

                                        numericSums[prop.Name] += val;
                                    }
                                    catch
                                    {
                                        displayValue = value.ToString();
                                    }
                                }
                                else if (type == typeof(DateTime))
                                {
                                    displayValue = ((DateTime)value).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    displayValue = value.ToString();
                                }
                            }

                            bool isRightAligned = IsNumericType(type);
                            table.Cell()
                                 .Element(isRightAligned ? CellStyleRight : CellStyle)
                                 .Text(text => text.Span(displayValue).FontSize(6));
                        }
                    }
                }


                if (_needcalculation)
                {
                    bool totalLabelPrinted = false;

                    foreach (var prop in properties)
                    {
                        if (prop.Name == "Code" || prop.Name == groupByPropertyName)
                            continue;

                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (!totalLabelPrinted)
                        {
                            table.Cell().Element(CellStyleOnlyTotal).Text("Total").FontSize(7).Bold();
                            totalLabelPrinted = true;
                        }
                        else if (numericSums.ContainsKey(prop.Name))
                        {
                            decimal totalVal = numericSums[prop.Name];
                            string total = IsFloatingPointType(type)
                                ? totalVal.ToString("N2", CultureInfo.InvariantCulture)
                                : totalVal.ToString("N2", CultureInfo.InvariantCulture);

                            table.Cell().Element(CellStyleTotal).Text(total).FontSize(7).Bold();
                        }
                        else
                        {
                            table.Cell().Element(CellStyleTotal).Text("").FontSize(7);
                        }
                    }
                }


                // ====== Styles ======
                static IContainer CellStyle(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignLeft();

                static IContainer CellStyleRight(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle().AlignRight();

                static IContainer CellStyleHeader(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten2).Padding(3).AlignMiddle().AlignLeft();

                static IContainer CellStyleHeaderRight(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten2).Padding(3).AlignMiddle().AlignRight();

                static IContainer CellStyleTotal(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignMiddle().AlignRight();

                static IContainer CellStyleOnlyTotal(IContainer container) =>
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.LightGreen.Lighten1).Padding(5).AlignMiddle().AlignRight();
            });
        }

        // Helper to detect numeric types

        private static bool IsFloatingPointType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        private static bool IsNumericType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal);
        }
    }
}
