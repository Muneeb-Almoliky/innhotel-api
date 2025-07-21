using InnHotel.UseCases.Guests.Get;
using InnHotel.Web.Common;
using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.Web.Guests;

/// <summary>
/// Get a Guest by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Guest record.
/// </remarks>
public class GetById(IMediator _mediator)
  : Endpoint<GetGuestByIdRequest, object>
{
  public override void Configure()
  {
    Get(GetGuestByIdRequest.Route);
    Summary(s =>
    {
      s.Summary = "Get Guest by ID";
      s.Description = "Returns a guest record matching the provided ID if found";
    });
  }

  public override async Task HandleAsync(GetGuestByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetGuestQuery(request.GuestId);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendAsync(
        new FailureResponse(404, $"Guest with ID {request.GuestId} not found"),
        statusCode: 404,
        cancellation: cancellationToken
      );
      return;
    }

    if (result.IsSuccess)
    {
      var guest = result.Value;

      var guestRecord = new GuestRecord(
    guest.Id,
    guest.FirstName,
    guest.LastName,
    guest.Gender, // بدون ToString()
    guest.IdProofType,
    guest.IdProofNumber,
    guest.Email,
    guest.Phone,
    guest.Address
);


      await SendOkAsync(guestRecord, cancellationToken);
      return;
    }

    await SendAsync(
      new FailureResponse(500, "An unexpected error occurred."),
      statusCode: 500,
      cancellation: cancellationToken
    );
  }
}
