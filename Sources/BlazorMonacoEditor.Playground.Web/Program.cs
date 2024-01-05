using BlazorMonacoEditor;
using BlazorMonacoEditor.Playground.Pages;
using BlazorMonacoEditor.Roslyn;

var builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder
        .ConfigureServices(ConfigureServices)
        .Configure((context, applicationBuilder) => Configure(context.HostingEnvironment, applicationBuilder))
        .UseStaticWebAssets();
});

await builder.Build().RunAsync();


void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddServerSideBlazor().AddCircuitOptions(options => options.DetailedErrors = true);
    services.AddMonaco();
    services.AddMonacoRoslynCompletionProvider();
    services.AddScoped<IndexViewModel>();
}

void Configure(IWebHostEnvironment env, IApplicationBuilder app)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
    });
}