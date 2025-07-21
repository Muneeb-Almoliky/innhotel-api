using Ardalis.Result.AspNetCore;
using InnHotel.UseCases.Guests.Create;
using InnHotel.Core.GuestAggregate.ValueObjects; // للوصول إلى Gender و IdProofType enums

namespace InnHotel.Web.Guests;

/// <summary>
/// Create a new Guest
/// </summary>
public class Create(IMediator _mediator)
  : Endpoint<CreateGuestRequest, CreateGuestResponse>
{
  public override void Configure()
  {
    Post(CreateGuestRequest.Route);
    Summary(s =>
    {
      s.ExampleRequest = new CreateGuestRequest
      {
        FirstName = "Jane",
        LastName = "Doe",
        Gender = "Female",
        IdProofType = "Passport",
        IdProofNumber = "X1234567",
        Email = "jane.doe@example.com",
        Phone = "+1234567890",
        Address = "123 Main St"
      };
    });
  }

  public override async Task HandleAsync(
    CreateGuestRequest request,
    CancellationToken cancellationToken)
  {
    // ✅ تحويل النصوص إلى enum مع التحقق
    if (!Enum.TryParse<Gender>(request.Gender, ignoreCase: true, out var gender))
    {
      AddError("Invalid gender value.");
      await SendErrorsAsync();
      return;
    }

    if (!Enum.TryParse<IdProofType>(request.IdProofType, ignoreCase: true, out var idProofType))
    {
      AddError("Invalid ID proof type value.");
      await SendErrorsAsync();
      return;
    }

    // ✅ إرسال الأمر باستخدام القيم النصية الأصلية (لأن command مازال يستقبل string)
    var command = new CreateGuestCommand(
      request.FirstName!,
      request.LastName!,
      request.Gender!,
      request.IdProofType!,
      request.IdProofNumber!,
      request.Email,
      request.Phone,
      request.Address
    );

    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsSuccess)
    {
      // ✅ إعادة بناء الرد باستخدام القيم المحوّلة
      Response = new CreateGuestResponse(
        result.Value,
        request.FirstName!,
        request.LastName!,
        gender,
        idProofType,
        request.IdProofNumber!,
        request.Email,
        request.Phone,
        request.Address
      );
      return;
    }

    await SendResultAsync(result.ToMinimalApiResult());
  }
}
