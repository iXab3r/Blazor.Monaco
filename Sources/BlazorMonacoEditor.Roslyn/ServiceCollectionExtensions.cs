using BlazorMonacoEditor.Roslyn.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.TryAddScoped<IRoslynCompletionProvider, RoslynCompletionProvider>();
            return services;
        }
    }
}
