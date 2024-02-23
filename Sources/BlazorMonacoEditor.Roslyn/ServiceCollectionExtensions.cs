using BlazorMonacoEditor.Roslyn.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMonacoEditor.Roslyn
{
    /// <summary>
    /// Extension methods fo
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services for the Monaco component.
        /// </summary>
        public static IServiceCollection AddMonacoRoslyn(this IServiceCollection services)
        {
            services.AddScoped<IRoslynCompletionProvider, RoslynCompletionProvider>();
            return services;
        }
    }
}
