using InnHotel.Core.GuestAggregate;
using Ardalis.Result;

namespace InnHotel.UseCases.Guests.List;

public class ListGuestsHandler(IReadRepository<Guest> _repository)
  : IQueryHandler<ListGuestsQuery, Result<(List<GuestDTO> Items, int TotalCount)>>
{
  public async Task<Result<(List<GuestDTO> Items, int TotalCount)>> Handle(ListGuestsQuery request, CancellationToken cancellationToken)
  {
    var totalCount = await _repository.CountAsync(cancellationToken);

    var guests = await _repository.ListAsync(cancellationToken);
    var pagedGuests = guests
      .Skip((request.PageNumber - 1) * request.PageSize)
      .Take(request.PageSize);

    var guestDtos = pagedGuests.Select(entity => new GuestDTO(
      entity.Id,
      entity.FirstName,
      entity.LastName,
      entity.Gender,                      // ✅ تمت إضافته هنا
      entity.IdProofType,
      entity.IdProofNumber,
      entity.Email,
      entity.Phone,
      entity.Address ?? ""
    )).ToList();

    return (guestDtos, totalCount);
  }
}
