using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.TryAddScoped<MonacoInterop>();
            services.TryAddScoped<IMonacoInterop>(provider => provider.GetRequiredService<MonacoInterop>());

            return services;
        }
    }
}
