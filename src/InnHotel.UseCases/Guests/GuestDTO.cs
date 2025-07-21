using InnHotel.Core.GuestAggregate.ValueObjects;


namespace InnHotel.UseCases.Guests;

public record GuestDTO(
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
