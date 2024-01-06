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
            services.AddSingleton<RoslynCompletionProvider>();
            services.AddSingleton<IRoslynCompletionProvider>(provider => provider.GetRequiredService<RoslynCompletionProvider>()!);
            
            services.AddScoped<RoslynCompletionProviderController>();
            services.AddScoped<IRoslynCompletionProviderController>(provider => provider.GetRequiredService<RoslynCompletionProviderController>());
            return services;
        }
    }
}
