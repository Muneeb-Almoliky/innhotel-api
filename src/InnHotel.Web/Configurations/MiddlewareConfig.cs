using Ardalis.ListStartupServices;
using InnHotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.Web.Configurations;

public static class MiddlewareConfig
{
  public static async Task<WebApplication> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware();
    }
    else
    {
      app.UseDefaultExceptionHandler();
      app.UseHsts();
    }

    // wire up FastEndpoints + Swagger/static files
    app.UseFastEndpoints(c =>   c.Endpoints.RoutePrefix = "api") // set a global prefix for all endpoints
       .UseSwaggerGen();

    app.UseHttpsRedirection();

    // now seed
    await SeedDatabase(app);

    return app;
  }

  static async Task SeedDatabase(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
      var context = services.GetRequiredService<AppDbContext>();
      if (app.Environment.IsDevelopment())
        await context.Database.MigrateAsync();

      await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
  }
}
