namespace SadaqaAccounting.Api.Extensions
{
    public static class AppConfigureExtensions
    {
        public static WebApplication ConfigureCors(this WebApplication app, IConfiguration configuration)
        {
            app.UseCors("AllowAll");

            // Add hang fire dashboard
            app.UseHangfireDashboard();

            return app;
        }
    }
}