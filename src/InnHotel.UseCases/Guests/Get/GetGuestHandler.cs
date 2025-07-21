using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnHotel.Core.ContributorAggregate.Specifications;
using InnHotel.Core.ContributorAggregate;
using InnHotel.UseCases.Contributors.Get;
using InnHotel.UseCases.Contributors;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.GuestAggregate.Specifications;

namespace InnHotel.UseCases.Guests.Get;
public class GetGuestHandler(IReadRepository<Guest> _repository)
  : IQueryHandler<GetGuestQuery, Result<GuestDTO>>
{
  public async Task<Result<GuestDTO>> Handle(GetGuestQuery request, CancellationToken cancellationToken)
  {
    var spec = new GuestByIdSpec(request.guestId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new GuestDTO(
      entity.Id,
      entity.FirstName,
      entity.LastName,
      entity.Gender,
      entity.IdProofType,
      entity.IdProofNumber,
      entity.Email,
      entity.Phone,
      entity.Address ?? "");
  }
}
