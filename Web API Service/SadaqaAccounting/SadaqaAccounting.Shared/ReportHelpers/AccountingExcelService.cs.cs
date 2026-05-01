namespace SadaqaAccounting.Shared.ReportHelpers
{
    public class AccountingExcelService<T, F, Y> : BaseExcelService<T, F> where F : IFilterItem
    {
        private readonly List<Y> _secondTable;
        private readonly string _billTO;
        private readonly string _shippingTo;
        private readonly string _headerNumber;
        private readonly Dictionary<string, string> _summary;

        public AccountingExcelService(List<Y> secondTable, string billTo, string shippingTo, string headerNumber, Dictionary<string, string> summary)
        {
            _secondTable = secondTable;
            _billTO = billTo;
            _shippingTo = shippingTo;
            _headerNumber = headerNumber;
            _summary = summary;
        }

        public override void ComposeTable(ICollection<T> dataList)
        {
            ws.Cell(row,1).Value = _headerNumber;
            ws.Cell(row, 1).Style.Font.Bold=true;
            ws.Cell(row, 1).Style.Font.FontSize=20;
            row++;

            // 2. Bill To and Shipping To
            ws.Cell(row, 1).Value = $"Bill To : {_billTO}";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 3).Value = $"Shipping To :{_shippingTo}";
            ws.Cell(row, 3).Style.Font.Bold = true;

            foreach (var entry in _summary)
            {
                if(entry.Key != "Status")
                {
                    ws.Cell(row, 5).Value = $"{entry.Key} : {entry.Value}";
                    ws.Cell(row, 5).Style.Font.Bold = true;
                    row++;
                }
                else
                {
                    if(entry.Value == "Paid")
                    {
                        ws.Cell(row, 5).Value = $"{entry.Key} : {entry.Value}";
                        ws.Cell(row, 5).Style.Font.Bold = true;
                        ws.Cell(row, 5).Style.Font.FontColor = XLColor.Green;
                        row++;
                    }
                    else
                    {
                        ws.Cell(row, 5).Value = $"{entry.Key} : {entry.Value}";
                        ws.Cell(row, 5).Style.Font.Bold = true;
                        ws.Cell(row, 5).Style.Font.FontColor = XLColor.Red;
                        row++;
                    }
                }

            }

            row++;

            var standardMain = new StandardTableExcelService<T, F>(true)
            {
                ws = ws,
                row = row
            };

            standardMain.ComposeTable(dataList);
            row = standardMain.row + 2;

            ws.Cell(row, 1).Value = "Transactions For this Invoice";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 14;
            row++;

            var standardSecond = new StandardTableExcelService<Y, F>(true)
            {
                ws = ws,
                row = row
            };

            standardSecond.ComposeTable(_secondTable);
            row = standardSecond.row;
        }
    }
}