using AgronicaNetCore.Base.Services.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgronicaCoreAPI.Extensions
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
