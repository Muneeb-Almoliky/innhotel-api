using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.Web.Guests;

/// <summary>
/// Response DTO returned after creating a Guest
/// </summary>
public class CreateGuestResponse
{
  public CreateGuestResponse(
        int id,
        string firstName,
        string lastName,
        Gender gender,
        IdProofType idProofType,
        string idProofNumber,
        string? email,
        string? phone,
        string? address)
  {
    Id = id;
    FirstName = firstName;
    LastName = lastName;
    Gender = gender;
    IdProofType = idProofType;
    IdProofNumber = idProofNumber;
    Email = email;
    Phone = phone;
    Address = address;
  }

  public int Id { get; init; }
  public string FirstName { get; init; } = default!;
  public string LastName { get; init; } = default!;
  public Gender Gender { get; init; }
  public IdProofType IdProofType { get; init; }
  public string IdProofNumber { get; init; } = default!;
  public string? Email { get; init; }
  public string? Phone { get; init; }
  public string? Address { get; init; }
}
