using Blazor.Monaco;

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
    services.AddServerSideBlazor();
    services.AddMonaco();
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