using Ardalis.Result;
using InnHotel.Core.AuthAggregate;
using InnHotel.Core.AuthAggregate.Specifications;
using InnHotel.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InnHotel.UseCases.Auth.Logout;

public class LogoutHandler(
    IRepository<RefreshToken> refreshTokenRepo,
    UserManager<ApplicationUser> userManager)
    : ICommandHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Result<bool>.NotFound("User not found");

        var spec = new RefreshTokenByTokenAndUserSpec(request.RefreshToken, request.UserId);
        var refreshToken = await refreshTokenRepo.FirstOrDefaultAsync(spec, cancellationToken);

        if (refreshToken is null)
            return Result<bool>.NotFound("Refresh token not found");

        await refreshTokenRepo.DeleteAsync(refreshToken, cancellationToken);

        return Result<bool>.Success(true);
    }
}