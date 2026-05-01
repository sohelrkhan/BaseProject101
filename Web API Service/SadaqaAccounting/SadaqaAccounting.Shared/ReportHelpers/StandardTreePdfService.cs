namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class StandardTreePdfService<T, F> : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly bool _isGrouped;
        public bool _needcalculation { get; set; }

        public StandardTreePdfService(bool needCalculation = false, bool isGrouped = false)
        {
            _needcalculation = needCalculation;
            _isGrouped = isGrouped;
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string groupByPropertyName = null)
        {

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var numericProperties = new List<PropertyInfo>();
            var numericSums = new Dictionary<string, decimal>();

            foreach (var prop in properties)
            {
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
                        columns.RelativeColumn();
                    }
                });

                table.Header(header =>
                {
                    foreach (var prop in properties)
                    {
                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        bool isNumeric = IsNumericType(type);

                        header.Cell()
                              .Element(isNumeric ? CellStyleHeaderRight : CellStyleHeader)
                              .Text(prop.Name.ToSentenceCase())
                              .SemiBold().FontSize(9);
                    }
                });
                // Data Rows
                foreach (var item in dataList)
                {
                    foreach (var prop in properties)
                    {
                        string displayValue = "";
                        var value = prop.GetValue(item);
                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (value != null)
                        {
                            if (prop.Name == "Name")
                            {
                                var treePrefix = item.GetType().GetProperty("TreePrefix")?.GetValue(item)?.ToString() ?? "";
                                displayValue = treePrefix + value.ToString();
                            }
                            else if (IsNumericType(type))
                            {
                                try
                                {
                                    decimal val = Convert.ToDecimal(value);
                                    displayValue = IsFloatingPointType(type) ? val.ToString("0.00") : val.ToString("0");
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
                        bool isNameColumn = prop.Name == "Name";

                        table.Cell()
                            .Element(e =>
                            {
                                if (isNameColumn)
                                {
                                    var level = (int?)item.GetType().GetProperty("Level")?.GetValue(item) ?? 0;
                                    return CellStyleIndented(e, level);
                                }
                                else if (isRightAligned)
                                    return CellStyleRight(e);
                                else
                                    return CellStyle(e);
                            })
                            .Text(text => text
                            .Span(displayValue)
                            .FontSize(8)
                            .FontFamily("Courier New") // Ensures tree lines align properly
                            );
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
                   container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.LightGreen.Lighten1).Padding(5).AlignMiddle().AlignCenter();

                static IContainer CellStyleIndented(IContainer container, int level) =>
                    container
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .PaddingLeft(10 + (level * 10)) // 10px base + 10px per level
                        .PaddingVertical(5)
                        .AlignMiddle()
                        .AlignLeft();

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
