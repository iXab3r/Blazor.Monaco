using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
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
            services.AddScoped<IMonacoInterop>(provider => provider.GetService<MonacoInterop>()!);

            return services;
        }
    }
}
