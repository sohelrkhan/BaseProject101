namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class AccountingPDFService<T, F, Y> : BasePDFService<T, F> where F : IFilterItem
    {
        private readonly List<Y> _secondTable;
        private readonly string _billTO;
        private readonly string _shippingTo;
        private readonly string _headerNumber;
        private readonly Dictionary<string, string> _summary;

        public AccountingPDFService(List<Y> secondTable, string billTo, string shippingTo, string headerNumber, Dictionary<string, string> summary)
        {
            _secondTable = secondTable;
            _billTO = billTo;
            _shippingTo = shippingTo;
            _headerNumber = headerNumber;
            _summary = summary;
        }

        public override void ComposeTable(IContainer container, ICollection<T> dataList, string? groupByPropertyName = null)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(2).PaddingBottom(2).Text(_headerNumber)
                    .FontSize(20)
                    .Bold()
                    .AlignLeft();

                column.Item().Element(row =>
                {
                    row.Row(rowItems =>
                    {
                        rowItems.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Bill To").Bold().FontSize(9);
                            col.Item().Text(_billTO).FontSize(9);
                        });

                        rowItems.ConstantItem(30); 

                        rowItems.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Shipping To").Bold().FontSize(9);
                            col.Item().Text(_shippingTo).FontSize(9);
                        });

                        rowItems.ConstantItem(30); 

                        rowItems.RelativeItem().Column(col =>
                        {
                            foreach (var entry in _summary)
                            {
                                AddLabeledRow(col, entry.Key, entry.Value);
                            }
                        });
                    });
                });

                column.Item().PaddingBottom(15);

                column.Item().Element(inner =>
                {
                    var standardService = new StandardTablePdfService<T, F>(true);
                    standardService.ComposeTable(inner, dataList);
                });

                column.Item().PaddingBottom(15);

                column.Item().PaddingBottom(5).Text("Transactions For this Invoice")
                    .FontSize(10)
                    .Bold()
                    .AlignLeft();

                column.Item().Element(inner =>
                {
                    var standardService = new StandardTablePdfService<Y, F>(true);
                    standardService.ComposeTable(inner, _secondTable);
                });
            });
        }

        private void AddLabeledRow(ColumnDescriptor col, string label, string value)
        {
            if (label != "Status")
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem(1).Text(label).FontSize(9).Bold();
                    row.ConstantItem(5).Text(":");
                    row.RelativeItem(2).Text(value ?? "-").FontSize(9);
                });
            }
            else
            {
                if (value == "Paid")
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(1).Text(label).FontSize(9).Bold();
                        row.ConstantItem(5).Text(":");
                        row.RelativeItem(2).Text(value ?? "-").FontSize(9).FontColor(Colors.Green.Darken1);
                    });
                }
                else
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(1).Text(label).FontSize(9).Bold();
                        row.ConstantItem(5).Text(":");
                        row.RelativeItem(2).Text(value ?? "-").FontSize(9).FontColor(Colors.Red.Darken1);
                    });
                }
            }
        }
    }
}