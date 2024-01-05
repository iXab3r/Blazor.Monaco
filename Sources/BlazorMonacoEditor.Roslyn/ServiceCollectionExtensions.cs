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
        public static IServiceCollection AddMonacoRoslynCompletionProvider(this IServiceCollection services)
        {
            services.AddSingleton<RoslynCompletionProvider>();
            services.AddSingleton<IRoslynCompletionProvider>(provider => provider.GetService<RoslynCompletionProvider>()!);

            return services;
        }
    }
}
