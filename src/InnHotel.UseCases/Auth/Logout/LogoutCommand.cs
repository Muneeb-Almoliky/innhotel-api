using InnHotel.Core.AuthAggregate;

namespace InnHotel.UseCases.Auth.Logout;

public record LogoutCommand(
    string UserId,
    string RefreshToken
) : ICommand<Result<bool>>;