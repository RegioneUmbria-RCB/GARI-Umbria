using AgronicaNetCore.Base.Services.Security;

namespace AgronicaNetCoreApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder Init(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<ISecurityService>();

            return app;
        }
    }
}
