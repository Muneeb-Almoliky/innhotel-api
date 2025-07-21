using InnHotel.UseCases.Guests.List;
using InnHotel.Web.Common;
using InnHotel.Core.GuestAggregate.ValueObjects; // لدعم enum Gender و IdProofType

namespace InnHotel.Web.Guests;

/// <summary>
/// List all Guests with pagination support.
/// </summary>
/// <remarks>
/// Returns a paginated list of Guest records.
/// </remarks>
public class List(IMediator _mediator)
    : Endpoint<PaginationRequest, object>
{
  public override void Configure()
  {
    Get(ListGuestsRequest.Route);
    Summary(s =>
    {
      s.Summary = "Get paginated list of guests";
      s.Description = "Returns a paginated list of guest records with optional page number and size parameters";
    });
  }

  public override async Task HandleAsync(PaginationRequest request, CancellationToken cancellationToken)
  {
    var query = new ListGuestsQuery(request.PageNumber, request.PageSize);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsSuccess)
    {
      var (items, totalCount) = result.Value;

      var guestRecords = items.Select(g =>
          new GuestRecord(
              g.Id,
              g.FirstName,
              g.LastName,
              g.Gender,
              g.IdProofType,
              g.IdProofNumber,
              g.Email,
              g.Phone,
              g.Address ?? "")
      ).ToList();

      var response = new PagedResponse<GuestRecord>(
          guestRecords,
          totalCount,
          request.PageNumber,
          request.PageSize
      );

      await SendOkAsync(response, cancellationToken);
      return;
    }

    await SendAsync(new FailureResponse(500, "An unexpected error occurred."),
        statusCode: 500,
        cancellation: cancellationToken);
  }
}
