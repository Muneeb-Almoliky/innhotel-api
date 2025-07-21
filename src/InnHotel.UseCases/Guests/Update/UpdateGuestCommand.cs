using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.UseCases.Guests.Update;

public record UpdateGuestCommand(
    int GuestId,
    string FirstName,
    string LastName,
    Gender Gender,
    IdProofType IdProofType,
    string IdProofNumber,
    string? Email,
    string? Phone,
    string? Address
) : ICommand<Result<GuestDTO>>
;
