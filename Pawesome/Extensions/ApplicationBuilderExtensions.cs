namespace Pawesome.Extensions;

/// <summary>
/// Extension methods for WebApplication to configure the HTTP request pipeline
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with common middleware components
    /// </summary>
    /// <param name="app">The WebApplication instance to configure</param>
    /// <returns>The configured WebApplication instance</returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        return app;
    }
}
