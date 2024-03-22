using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PushNotifications.Api
{
    /// <summary>
    /// Extensions related to ProblemDetails 
    /// </summary>
    public static class ProblemDetailsExtensions
    {
        /// <summary>
        /// Configures all custom behavior or settings needed for using ProblemDetails
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomProblemDetails(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.Add(new ServiceDescriptor(typeof(ApiResponse), typeof(ApiResponse), ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(ApiCqrsResponse), typeof(ApiCqrsResponse), ServiceLifetime.Singleton));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ctx => new AspModelValidation();
            });

            services.AddSingleton<ApiResponse>();
        }
    }
}
