namespace SadaqaAccounting.Shared.ReportHelpers
{
    public abstract class BasePDFService<T, F> where F : IFilterItem
    {
        public BasePDFService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GeneratePDF(string title, string companyName, string companyAddress, string email,
            string website, string phoneNumber, string createdByName,
            ICollection<F> filters, ICollection<T> dataList,
            byte[]? logoImage = null, byte[]? qrCodeImage = null, string mode = "p")
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(mode == "p" ? PageSizes.A4.Portrait() : PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Background().Element(bg =>
                    {
                        if (logoImage != null && logoImage.Length > 0)
                        {
                            bg.PaddingTop(100).AlignCenter().AlignMiddle()
                              .Height(200).Width(200)
                              .Image(MakeImageSemiTransparent(logoImage));
                        }
                    });


                    page.Content().Element(content =>
                    {
                        content.Column(column =>
                        {
                            column.Item().PaddingLeft(20).PaddingRight(20).Element(ComposeMainHeader(companyName, logoImage, qrCodeImage));
                            column.Item().PaddingLeft(20).Text(companyName).FontSize(10).SemiBold();
                            column.Item().PaddingLeft(20).PaddingBottom(2).Text(companyAddress).FontSize(8);
                            column.Item().PaddingTop(5).PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2);
                            column.Item().Element(ComposeHeader(title, filters, createdByName));
                            column.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten2);
                            if (!string.IsNullOrEmpty(title))
                            {
                                column.Item().AlignCenter().PaddingTop(5).PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2)
                                                               .Background(Colors.Grey.Lighten3)
                                                               .PaddingVertical(4)
                                                               .PaddingHorizontal(6)
                                                               .Shrink().Text(title).FontSize(10).Bold();
                            }
                            column.Item().Padding(3).Element(container => ComposeTable(container, dataList));
                        });
                    });

                    page.Footer().Column(column =>
                    {
                        column.Item().PaddingLeft(20).PaddingRight(20).Element(ComposeFooter(email, website, phoneNumber));
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> ComposeFooter(string email, string website, string phoneNumber) => container =>
        {
            container.PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
            {
                row.RelativeColumn().Column(column =>
                {
                    column.Item().Padding(10).AlignCenter().Text("This is a system-generated report and does not require a signature. To verify its authenticity, please scan the QR code in the header.")
                        .Italic().FontSize(7).FontColor(Colors.Red.Darken1).Underline();

                    column.Item().Padding(20).AlignCenter().Text($"Website : {website}      Email : {email}     Mobile : {phoneNumber}")
                        .FontSize(6).Italic().FontColor(Colors.Grey.Darken1);
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

        private static Action<IContainer> ComposeMainHeader(string companyName, byte[] logoImage, byte[] qrCodeImage) => container =>
        {
            container.Row(row =>
            {
                row.RelativeColumn().Column(column =>
                {
                    column.Item().Row(innerRow =>
                    {
                        innerRow.ConstantColumn(70).Element(e =>
                        {
                            if (logoImage != null && logoImage.Length > 0)
                                e.Image(logoImage, ImageScaling.FitArea);
                            else
                                e.AlignMiddle().AlignCenter().Text("No Logo").FontSize(20).Italic();
                        });
                    });
                });

                row.ConstantColumn(60).AlignRight().Element(e =>
                {
                    if (qrCodeImage != null && qrCodeImage.Length > 0)
                        e.Image(qrCodeImage, ImageScaling.FitArea);
                    else
                        e.AlignMiddle().AlignCenter().Text("No QR").FontSize(6).Italic();
                });
            });
        };


        private static Action<IContainer> ComposeHeader(string title, ICollection<F> filters, string createdbyName) => container =>
        {
            container.Row(row =>
            {
                row.RelativeColumn().Column(column =>
                {
                    foreach (var filter in filters)
                    {
                        if (filter.Key != "Company" && filter.Value != null && !string.IsNullOrEmpty(filter.Value))
                        {
                            column.Item().PaddingLeft(10).PaddingTop(5).Text($"{filter.Key} : {filter.Value}")
                                  .FontSize(9).AlignStart().FontColor(Colors.Black.Alpha);
                        }
                    }
                });

                row.ConstantColumn(200).Column(column =>
                {
                    column.Item().AlignLeft().PaddingTop(5).Text("").FontSize(10).Bold();
                });

                row.ConstantColumn(140).Column(column =>
                {
                    column.Item().AlignLeft().PaddingTop(5).PaddingRight(10).Text(x =>
                    {
                        x.Span("Created Date : ").FontSize(6);
                        x.Span(DateTime.Now.ToString("dd MMM yyyy hh:mm tt")).FontSize(6);
                    });
                    column.Item().AlignLeft().PaddingTop(5).PaddingRight(10).Text($"Created By : {createdbyName}").FontSize(6);
                });
            });
        };

        public static byte[] MakeImageSemiTransparent(byte[] imageBytes, float opacity = 0.1f)
        {
            using var ms = new MemoryStream(imageBytes);
            using var original = System.Drawing.Image.FromStream(ms);
            using var bmp = new Bitmap(original.Width, original.Height);

            using (var graphics = Graphics.FromImage(bmp))
            {
                var colormatrix = new System.Drawing.Imaging.ColorMatrix
                {
                    Matrix33 = opacity // Set alpha
                };

                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colormatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                graphics.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            using var outputMs = new MemoryStream();
            bmp.Save(outputMs, System.Drawing.Imaging.ImageFormat.Png);
            return outputMs.ToArray();
        }

        public abstract void ComposeTable(IContainer container, ICollection<T> dataList, string groupByPropertyName = null);
    }
}