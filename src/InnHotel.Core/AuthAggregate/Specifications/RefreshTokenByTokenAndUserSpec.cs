using Ardalis.Specification;

namespace InnHotel.Core.AuthAggregate.Specifications;

public sealed class RefreshTokenByTokenAndUserSpec : Specification<RefreshToken>
{
    public RefreshTokenByTokenAndUserSpec(string token, string userId)
    {
        Query
            .Where(x => x.Token == token && x.UserId == userId);
    }
} 