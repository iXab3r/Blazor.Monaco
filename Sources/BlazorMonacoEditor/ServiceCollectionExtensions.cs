using BlazorMonacoEditor.Interop;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMonacoEditor
{
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
