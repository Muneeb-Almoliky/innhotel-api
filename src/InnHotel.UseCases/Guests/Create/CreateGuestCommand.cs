namespace InnHotel.UseCases.Guests.Create;

public record CreateGuestCommand(
    string FirstName,
    string LastName,
    string Gender,
    string IdProofType,
    string IdProofNumber,
    string? Email,
    string? Phone,
    string? Address
) : Ardalis.SharedKernel.ICommand<Result<int>>;
