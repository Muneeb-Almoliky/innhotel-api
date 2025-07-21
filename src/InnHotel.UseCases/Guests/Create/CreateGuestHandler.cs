using Ardalis.Result;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.GuestAggregate.Specifications;
using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.UseCases.Guests.Create;

internal class CreateGuestHandler(IRepository<Guest> _repository)
  : ICommandHandler<CreateGuestCommand, Result<int>>
{
  public async Task<Result<int>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
  {
    // تحقق من وجود النزيل بناءً على رقم إثبات الهوية
    var spec = new GuestByProofNumberSpec(request.IdProofNumber);
    var exists = await _repository.AnyAsync(spec, cancellationToken);
    if (exists)
    {
      return Result.Conflict("A guest with that ID proof number already exists.");
    }

    // تحويل القيم النصية إلى Enums
    if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
    {
      return Result.Invalid(new[] { new ValidationError("Gender", "Invalid gender value.") });
    }

    if (!Enum.TryParse<IdProofType>(request.IdProofType, true, out var idProofType))
    {
      return Result.Invalid(new[] { new ValidationError("IdProofType", "Invalid ID proof type.") });
    }

    // ✅ إنشاء كائن النزيل بشكل صحيح
    var guest = new Guest(
        request.FirstName,
        request.LastName,
        gender,
        idProofType,
        request.IdProofNumber,
        request.Email,
        request.Phone,
        request.Address
    );

    var createdGuest = await _repository.AddAsync(guest, cancellationToken);

    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success(createdGuest.Id);
  }
}
