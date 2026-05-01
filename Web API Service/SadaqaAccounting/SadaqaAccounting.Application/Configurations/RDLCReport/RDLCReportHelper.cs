using SixLabors.ImageSharp.PixelFormats;
using System.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace SadaqaAccounting.Application.Configurations.RDLCReport
{
    public static class RdlcReportHelper
    {
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static DataTable ObjectToDataTable<T>(T item)
        {
            var table = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            var values = new object[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null) ?? DBNull.Value;
            }

            table.Rows.Add(values);
            return table;
        }

        /// <summary>
        /// Takes a combination of lists or objects and returns a DataSet with one DataTable per item.
        /// </summary>
        public static DataSet ToDataSet(params object[] sources)
        {
            var dataset = new DataSet();

            foreach (var source in sources)
            {
                if (source == null)
                    continue;

                var type = source.GetType();
                string tableName = type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                    ? type.GetGenericArguments()[0].Name
                    : type.Name;

                DataTable table;

                if (source is IEnumerable<object> list && type.IsGenericType)
                {
                    var genericType = type.GetGenericArguments()[0];
                    var method = typeof(RdlcReportHelper).GetMethod(nameof(ListToDataTable))!
                        .MakeGenericMethod(genericType);
                    table = (DataTable)method.Invoke(null, new object[] { source })!;
                }
                else
                {
                    var method = typeof(RdlcReportHelper).GetMethod(nameof(ObjectToDataTable))!
                        .MakeGenericMethod(type);
                    table = (DataTable)method.Invoke(null, new object[] { source })!;
                }

                if (dataset.Tables.Contains(tableName))
                {
                    int i = 1;
                    while (dataset.Tables.Contains(tableName + "_" + i))
                        i++;

                    table.TableName = tableName + "_" + i;
                }

                dataset.Tables.Add(table);
            }

            return dataset;
        }

        public static async Task<string> GenerateQRCodeImageBase64(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Text cannot be null for QR code generation.");

            using var qrGenerator = new QRCodeGenerator();
            using var data = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(data);
            byte[] qrBytes = qrCode.GetGraphic(20);
            return Convert.ToBase64String(qrBytes);
        }

        //public static async Task<string> FromUrlAsLowVizBase64Async(string url, float alpha = 0.10f, float lightenBy = 0.50f)
        //{
        //    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
        //        (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        //        throw new ArgumentException("URL must start with http:// or https://", nameof(url));

        //    await using var stream = await new HttpClient().GetStreamAsync(uri);
        //    using var img = await Image.LoadAsync<Rgba32>(stream);

        //    img.Mutate(ctx =>
        //    {
        //        ctx.Opacity(alpha);
        //        var whiteOverlay = new Image<Rgba32>(img.Width, img.Height, Color.White);
        //        ctx.DrawImage(whiteOverlay, new GraphicsOptions
        //        {
        //            BlendPercentage = lightenBy,
        //            AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver
        //        });
        //    });

        //    await using var ms = new MemoryStream();
        //    await img.SaveAsync(ms, new PngEncoder());
        //    return Convert.ToBase64String(ms.ToArray());
        //}

        public static async Task<string> FromUrlAsLowVizBase64Async(string url, float alpha = 0.10f, float lightenBy = 0.50f)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("URL must start with http:// or https://", nameof(url));

            // Create HttpClient with proper configuration
            using var handler = new HttpClientHandler
            {
                // Bypass SSL validation for localhost (DEVELOPMENT ONLY!)
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var httpClient = new HttpClient(handler);

            // If your API uses authentication, add headers here:
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

            try
            {
                await using var stream = await httpClient.GetStreamAsync(uri);
                using var img = await Image.LoadAsync<Rgba32>(stream);

                img.Mutate(ctx =>
                {
                    ctx.Opacity(alpha);
                    using var whiteOverlay = new Image<Rgba32>(img.Width, img.Height, Color.White);
                    ctx.DrawImage(whiteOverlay, new GraphicsOptions
                    {
                        BlendPercentage = lightenBy,
                        AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver
                    });
                });

                await using var ms = new MemoryStream();
                await img.SaveAsync(ms, new PngEncoder());
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch image from URL: {url}. Status: {ex.StatusCode}", ex);
            }
        }
    }
}