using eVote360.Core.Application.Interfaces.Services;
using eVote360.Infrastructure.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOcrService, OcrService>();
        }
    }
}