namespace SadaqaAccounting.Api.Extensions
{
    public static class CompressionServiceExtensions
    {
        public static IServiceCollection AddCustomResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json", "application/javascript", "text/css", "image/svg+xml" });
                options.Providers.Add<BrotliCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });

            return services;
        }
    }
}