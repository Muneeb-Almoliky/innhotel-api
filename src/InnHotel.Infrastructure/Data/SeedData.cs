using InnHotel.Core.BranchAggregate;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.RoomAggregate;
using InnHotel.Core.ReservationAggregate;
using InnHotel.Core.ServiceAggregate;
using InnHotel.Core.AuthAggregate;
using Microsoft.AspNetCore.Identity;
using InnHotel.Core.GuestAggregate.ValueObjects;
namespace InnHotel.Infrastructure.Data;

public static class SeedData
{
  // Constants for default credentials
  private const string SuperAdminEmail = "super@innhotel.com";
  private const string AdminEmail = "admin@innhotel.com";

  // Application data entities
  public static readonly Branch MainBranch = new("Main Hotel", "123 Seaside Boulevard");
  public static readonly Branch DowntownBranch = new("Downtown Suites", "456 City Center Road");

  public static readonly RoomType StandardRoom = new(
      branchId: 1,
      name: "Standard",
      basePrice: 99.99m,
      capacity: 2,
      description: "Comfortable standard room with queen bed");

  public static readonly RoomType Suite = new(
      branchId: 1,
      name: "Suite",
      basePrice: 199.99m,
      capacity: 4,
      description: "Luxurious suite with king bed and ocean view");

  public static readonly RoomType DeluxeRoom = new(
      branchId: 2,
      name: "Deluxe Room",
      basePrice: 299.99m,
      capacity: 4,
      description: "Luxurious suite with king bed and ocean view");

  public static readonly RoomType DeluxeSuite = new(
      branchId: 2,
      name: "Deluxe Suite",
      basePrice: 399.99m,
      capacity: 4,
      description: "Luxurious suite with king bed and ocean view");

  public static readonly Guest TestGuest = new(
        firstName: "John",
        lastName:  "Doe",
        gender:    Gender.Male,
        idProofType: IdProofType.Passport,
        idProofNumber: "AB1234567",
        email:     "john.doe@example.com",
        phone:     "+1234567890",
        address:   "123 Main Street");

  public static async Task InitializeAsync(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var services = scope.ServiceProvider;

    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("SeedData");
    var config = services.GetRequiredService<IConfiguration>();

    try
    {
      var context = services.GetRequiredService<AppDbContext>();
      await SeedApplicationDataAsync(context);

      await SeedIdentityAsync(services, config);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred while seeding the database");
      throw;
    }
  }

  private static async Task SeedApplicationDataAsync(AppDbContext context)
  {
    if (await context.Branches.AnyAsync()) return;

    context.Branches.AddRange(MainBranch, DowntownBranch);
    await context.SaveChangesAsync();

    context.RoomTypes.AddRange(StandardRoom, Suite);
    await context.SaveChangesAsync();

    var rooms = new List<Room>
        {
            new(MainBranch.Id, StandardRoom.Id, "101", RoomStatus.Available, 1),
            new(MainBranch.Id, StandardRoom.Id, "102", RoomStatus.Available, 1),
            new(MainBranch.Id, Suite.Id, "201", RoomStatus.Available, 2)
        };
    context.Rooms.AddRange(rooms);

    context.Guests.Add(TestGuest);
    await context.SaveChangesAsync();

    var breakfast = new Service(MainBranch.Id, "Breakfast", 20.00m, "Continental breakfast");
    var spa = new Service(DowntownBranch.Id, "Spa", 50.00m, "Relaxing spa session");
    context.Services.AddRange(breakfast, spa);
    await context.SaveChangesAsync();

    var reservation = new Reservation(
        TestGuest.Id,
        DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
        DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
        ReservationStatus.Confirmed
        );

    context.Reservations.Add(reservation);
    await context.SaveChangesAsync();

    reservation.AddRoom(rooms[0].Id, 99.99m);
    reservation.AddService(breakfast.Id, 2, breakfast.Price);

    await context.SaveChangesAsync();
  }

  private static async Task SeedIdentityAsync(
      IServiceProvider services,
      IConfiguration config)
  {
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed roles
    foreach (var role in new[] { Roles.SuperAdmin, Roles.Admin, Roles.Receptionist, Roles.Housekeeping })
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        await roleManager.CreateAsync(new IdentityRole(role));
      }
    }

    // Seed Super Admin from configuration
    var superAdmin = await SeedUserAsync(
        userManager,
        config["SeedUsers:SuperAdmin:Email"] ?? SuperAdminEmail,
        config["SeedUsers:SuperAdmin:Password"] ?? "Sup3rP@ssword!",
        new[] { Roles.SuperAdmin, Roles.Admin });

    // Seed Admin from configuration
    var admin = await SeedUserAsync(
        userManager,
        config["SeedUsers:Admin:Email"] ?? AdminEmail,
        config["SeedUsers:Admin:Password"] ?? "Adm1nP@ssword!",
        new[] { Roles.Admin });
  }

  private static async Task<ApplicationUser?> SeedUserAsync(
      UserManager<ApplicationUser> userManager,
      string email,
      string password,
      string[] roles)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user != null) return user;

    user = new ApplicationUser
    {
      UserName = email,
      Email = email,
      EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded) return null;

    foreach (var role in roles)
    {
      await userManager.AddToRoleAsync(user, role);
    }

    return user;
  }
 
}
