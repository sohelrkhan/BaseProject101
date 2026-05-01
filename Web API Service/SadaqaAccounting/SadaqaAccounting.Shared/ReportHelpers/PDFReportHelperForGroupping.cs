namespace SadaqaAccounting.Shared
{
    public class PDFReportHelperForGroupping<T, F> where F : IFilterItem
    {
        public PDFReportHelperForGroupping()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [Obsolete]
        public byte[] GeneratePDF(string title, string companyName, string companyAddress, string email, string website, string phoneNumber, string createdByName,
            ICollection<F> filters, ICollection<T> dataList, byte[]? logoImage = null, byte[]? qrCodeImage = null)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Content().Element(content =>
                    {
                        content
                            .Column(column =>
                            {
                                column.Item().
                                PaddingLeft(20).PaddingRight(20).Element(ComposeMainHeader(companyName, logoImage, qrCodeImage));
                                column.Item().PaddingLeft(20).
                                      Text(companyName).FontSize(10).SemiBold();
                                column.Item().PaddingLeft(20).PaddingBottom(2).
                                      Text(companyAddress).FontSize(8);
                                column.Item().PaddingTop(5).PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2);
                                column.Item().Element(ComposeHeader(title, filters, createdByName));
                                column.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten2);
                                column.Item().Element(ComposeGroupedTablesByTeam(dataList));
                            });
                    });

                    page.Footer().Column(column =>
                    {
                        column.Item().
                                PaddingLeft(20).PaddingRight(20).Element(ComposeFooter(email, website, phoneNumber));
                    });
                });
            });

            //document.ShowInCompanion();
            return document.GeneratePdf();
        }

        [Obsolete]
        private static Action<IContainer> ComposeFooter(string email, string website, string phoneNumber) => container =>
        {
            container.PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
            {
                row.RelativeColumn().Column(column =>
                {
                    //column.Item().PaddingTop(10).PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2);
                    column.Item().Padding(10).AlignCenter().Text("This is a system-generated report and does not require a signature. To verify its authenticity, please scan the QR code in the header.")
                    .Italic()
                    .FontSize(7)
                    .FontColor(Colors.Red.Darken1)
                    .Underline();

                    column.Item().Padding(20).AlignCenter().Text($"Website : {website}      Email : {email}     Mobile : {phoneNumber}")
                    .FontSize(6)
                    .Italic()
                    .FontColor(Colors.Grey.Darken1);
                });

                row.ConstantColumn(60).AlignRight().Column(column =>
                {
                    column.Item().PaddingTop(30).AlignRight().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            });
        };

        [Obsolete]
        private static Action<IContainer> ComposeMainHeader(string companyName, byte[] logoImage, byte[] qrCodeImage) => container =>
        {
            container.Row(row =>
            {
                row.RelativeColumn().Column(column =>
                {
                    column.Item().Row(innerRow =>
                    {
                        innerRow.ConstantColumn(70).Image(logoImage, ImageScaling.FitArea);
                    });
                });

                row.ConstantColumn(60).AlignRight().Image(qrCodeImage, ImageScaling.FitArea);
            });
        };

        [Obsolete]
        private static Action<IContainer> ComposeHeader(string title, ICollection<F> filters, string createdbyName) => container =>
        {
            container.Row(row =>
            {
                row.RelativeColumn().Column(column =>
                {
                    foreach (var filter in filters)
                    {
                        var textvalue = $"{filter.Key} : {filter.Value}";
                        if (filter.Value != null)
                        {
                            if (filter.Key != "Company")
                            {
                                column.Item().PaddingLeft(10).PaddingTop(5).Text(textvalue).FontSize(9).AlignStart().FontColor(Colors.Black.Alpha);
                            }
                        }
                    }
                });

                row.ConstantColumn(200).Column(column =>
                {
                    column.Item().AlignLeft().PaddingTop(5).Text(title).FontSize(10).Bold();
                });

                row.ConstantColumn(140).Column(column =>
                {
                    column.Item().AlignLeft().PaddingTop(5).PaddingRight(10).Text(x =>
                    {
                        x.Span("Created Date : ").FontSize(6);
                        x.Span(DateTime.Now.ToString()).FontSize(6); ;
                    });
                    column.Item().AlignLeft().PaddingTop(5).PaddingRight(10).Text($"Created By : {createdbyName}").FontSize(6);
                });
            });

        };

        [Obsolete]
        private static Action<IContainer> ComposeTable(ICollection<T> dataList) => container =>
        {
            var properties = typeof(T).GetProperties();

            container
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "Code")
                        {
                            columns.RelativeColumn();
                        }
                    }
                });

                table.Header(header =>
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "Code")
                        {
                            header.Cell().Element(CellStyleHeader).Text(prop.Name.ToSentenceCase()).SemiBold().FontSize(9);
                        }
                    }
                });

                foreach (var item in dataList)
                {
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item)?.ToString() ?? string.Empty;

                        if (prop.Name == "Photo")
                        {
                            table.Cell().Element(CellStyle).Image((byte[])prop.GetValue(item), ImageScaling.FitArea);
                        }
                        else if (prop.Name != "Code")
                        {
                            table.Cell().Element(CellStyle).Text(text =>
                            {
                                text.Span(value).FontSize(6);
                            });
                        }
                    }
                }

                static IContainer CellStyle(IContainer container) =>
                    container
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Padding(5)
                        .AlignMiddle()
                        .AlignCenter();

                static IContainer CellStyleHeader(IContainer container) =>
                    container
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Background(Colors.Grey.Lighten2)
                        .Padding(3)
                        .AlignMiddle()
                        .AlignCenter();
            });
        };


        [Obsolete]
        private static Action<IContainer> ComposeGroupedTablesByTeam(ICollection<T> dataList) => container =>
        {
            var properties = typeof(T).GetProperties();
            var teamNameProp = properties.FirstOrDefault(p => p.Name == "TeamName");

            if (teamNameProp == null)
                throw new InvalidOperationException("Property 'TeamName' not found on type " + typeof(T).Name);

            var groupedData = dataList
                .Where(x => teamNameProp.GetValue(x) != null)
                .GroupBy(x => teamNameProp.GetValue(x).ToString())
                .OrderBy(g => g.Key);

            container.Column(column =>
            {
                foreach (var group in groupedData)
                {
                    column.Item().PaddingTop(8).Background(Colors.Grey.Lighten1).Text($" Team  : {group.Key}").FontSize(12).Bold();

                    column.Item().Element(ComposeTable(group.ToList()));
                }
            });
        };
    }
}