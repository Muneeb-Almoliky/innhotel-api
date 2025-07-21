using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.Web.Guests;

public record GuestRecord(
    int Id,
    string FirstName,
    string LastName,
    Gender Gender,
    IdProofType IdProofType,
    string IdProofNumber,
    string? Email,
    string? Phone,
    string? Address
);
