using InnHotel.Core.Interfaces;
using InnHotel.Core.Services;
using InnHotel.Infrastructure.Data;
using InnHotel.Infrastructure.Data.Queries;
using InnHotel.UseCases.Contributors.List;


namespace InnHotel.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    string? connectionString = config.GetConnectionString("PostgreSQLConnection");
    Guard.Against.Null(connectionString);

    services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(connectionString, o =>
        o.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
           .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
           .AddScoped<IDeleteContributorService, DeleteContributorService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IDeleteGuestService, DeleteGuestService>()
            .AddScoped<IDeleteBranchService, DeleteBranchService>()
            .AddScoped<IDeleteRoomService, DeleteRoomService>()
            .AddScoped<IDeleteEmployeeService, DeleteEmployeeService>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}

