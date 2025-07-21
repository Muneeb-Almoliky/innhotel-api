using InnHotel.UseCases.Guests.Update;
using InnHotel.Web.Common;
using InnHotel.Core.GuestAggregate.ValueObjects; // لدعم Gender و IdProofType enums

namespace InnHotel.Web.Guests;

/// <summary>
/// Update an existing Guest.
/// </summary>
/// <remarks>
/// Update an existing Guest by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<UpdateGuestRequest, object>
{
  public override void Configure()
  {
    Put(UpdateGuestRequest.Route);
    Summary(s =>
    {
      s.Summary = "Update a guest";
      s.Description = "Update guest details including gender and ID proof";
    });
  }

  public override async Task HandleAsync(
      UpdateGuestRequest request,
      CancellationToken cancellationToken)
  {
    // ✅ نحاول تحويل النصوص إلى Enum
    if (!Enum.TryParse<Gender>(request.Gender, ignoreCase: true, out var gender))
    {
      await SendAsync(new FailureResponse(400, "Invalid gender value."), statusCode: 400, cancellation: cancellationToken);
      return;
    }

    if (!Enum.TryParse<IdProofType>(request.IdProofType, ignoreCase: true, out var idProofType))
    {
      await SendAsync(new FailureResponse(400, "Invalid ID proof type value."), statusCode: 400, cancellation: cancellationToken);
      return;
    }

    var command = new UpdateGuestCommand(
     request.GuestId,
     request.FirstName,
     request.LastName,
     Enum.Parse<Gender>(request.Gender, true),
     Enum.Parse<IdProofType>(request.IdProofType, true),
     request.IdProofNumber,
     request.Email,
     request.Phone,
     request.Address
 );

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendAsync(
          new FailureResponse(404, $"Guest with ID {request.GuestId} not found"),
          statusCode: 404,
          cancellation: cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Error)
    {
      await SendAsync(
          new FailureResponse(400, result.Errors.First()),
          statusCode: 400,
          cancellation: cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      var guestRecord = new GuestRecord(
          result.Value.Id,
          result.Value.FirstName,
          result.Value.LastName,
          result.Value.Gender,
          result.Value.IdProofType,
          result.Value.IdProofNumber,
          result.Value.Email,
          result.Value.Phone,
          result.Value.Address ?? ""
      );

      await SendAsync(new { status = 200, message = "Guest updated successfully", data = guestRecord },
          statusCode: 200,
          cancellation: cancellationToken);
      return;
    }

    await SendAsync(
        new FailureResponse(500, "An unexpected error occurred."),
        statusCode: 500,
        cancellation: cancellationToken);
  }
}
