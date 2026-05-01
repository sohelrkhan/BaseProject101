namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class SummaryAndTablePdfService<T, F, H> : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly bool _needCalculation;
        private readonly H _headerInfo;

        public SummaryAndTablePdfService(H headerInfo, bool needCalculation = false)
        {
            _headerInfo = headerInfo;
            _needCalculation = needCalculation;
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string unused = null)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(10).Element(headerContainer =>
                {
                    headerContainer.Column(headerCol =>
                    {
                        var headerProps = typeof(H).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                        int totalLength = headerProps.Sum(p =>
                        {
                            var value = p.GetValue(_headerInfo)?.ToString()?.Length ?? 0;
                            var labelLength = p.Name.Length;
                            return labelLength + value;
                        });

                        double avgLength = (double)totalLength / headerProps.Length;

                        int propsPerRow = avgLength switch
                        {
                            <= 20 => 7,
                            <= 40 => 4,
                            <= 60 => 3,
                            _ => 1
                        };


                        for (int i = 0; i < headerProps.Length; i += propsPerRow)
                        {
                            headerCol.Item().Element(rowContainer =>
                            {
                                rowContainer
                                .Padding(4)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Background(Colors.Grey.Lighten3)
                                .Row(row =>
                                {
                                    row.Spacing(5);

                                    int j;
                                    for (j = i; j < i + propsPerRow && j < headerProps.Length; j++)
                                    {
                                        var prop = headerProps[j];
                                        var label = prop.Name.ToSentenceCase();
                                        var value = GetValue(_headerInfo, prop);

                                        row.RelativeItem((float)0.1).Element(cell =>
                                                                         cell
                                                                             .PaddingVertical(1)
                                                                             .PaddingHorizontal(2)
                                                                             .Text(text =>
                                                                             {
                                                                                 text.Span($"{label}: ").FontSize(7);      
                                                                                 text.Span($"{value}").Bold().FontSize(7); 
                                                                             })
                                                                     );

                                    }

                                    for (; j < i + propsPerRow; j++)
                                    {
                                        row.RelativeItem((float)0.1).Element(cell =>
                                            cell
                                                .Text("")
                                        );
                                    }
                                });
                            });
                        }
                    });
                });



                column.Item().Element(inner =>
                {
                    var standardService = new StandardTablePdfService<T, F>(_needCalculation);
                    standardService.ComposeTable(inner, dataList);
                });
            });
        }

        private string GetValue(H headerObj, PropertyInfo property)
        {
            var value = property.GetValue(headerObj);
            return value switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm"),
                decimal d => d.ToString("N2"),
                bool b => b ? "Yes" : "No",
                _ => value?.ToString() ?? string.Empty
            };
        }
    }
}
