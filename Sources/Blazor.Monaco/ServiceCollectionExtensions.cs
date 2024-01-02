namespace Blazor.Monaco
{
    using Blazor.Monaco.Interop;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods fo
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services for the Monaco component.
        /// </summary>
        public static IServiceCollection AddMonaco(this IServiceCollection services)
        {
            services.AddScoped<MonacoInterop>();

            return services;
        }
    }
}
