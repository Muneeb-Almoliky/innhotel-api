using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.GuestAggregate.Specifications;
using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.UseCases.Guests.Update;

public class UpdateGuestHandler(IRepository<Guest> _repository)
    : ICommandHandler<UpdateGuestCommand, Result<GuestDTO>>
{
  public async Task<Result<GuestDTO>> Handle(UpdateGuestCommand request, CancellationToken cancellationToken)
  {
    // Retrieve the guest by ID
    var spec = new GuestByIdSpec(request.GuestId);
    var guest = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (guest == null)
      return Result<GuestDTO>.NotFound();

    // Check if ID proof number is already used by another guest
    var idProofSpec = new GuestByIdProofNumberSpec(request.IdProofNumber);
    var existingGuest = await _repository.FirstOrDefaultAsync(idProofSpec, cancellationToken);
    if (existingGuest != null && existingGuest.Id != request.GuestId)
    {
      return Result<GuestDTO>.Invalid(
          new ValidationError(nameof(request.IdProofNumber),
                               "ID proof number is already registered with another guest."));
    }

    // Update guest details
    guest.UpdateDetails(
        request.FirstName,
        request.LastName,
        request.Gender,
        request.IdProofType,
        request.IdProofNumber,
        request.Email,
        request.Phone,
        request.Address
    );

    try
    {
      await _repository.UpdateAsync(guest, cancellationToken);
    }
    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_guests_idproof_number") == true)
    {
      return Result<GuestDTO>.Invalid(
          new ValidationError(nameof(request.IdProofNumber),
                               "ID proof number is already registered with another guest."));
    }

    // Map to DTO
    var dto = new GuestDTO(
        guest.Id,
        guest.FirstName,
        guest.LastName,
        guest.Gender,
        guest.IdProofType,
        guest.IdProofNumber,
        guest.Email,
        guest.Phone,
        guest.Address ?? string.Empty
    );

    return Result.Success(dto);
  }
}
