using Ardalis.Result.AspNetCore;
using InnHotel.Core.AuthAggregate;
using InnHotel.UseCases.Auth.Refresh;
using InnHotel.Web.Helpers;
using Serilog;

namespace InnHotel.Web.Auth;

public class RefreshToken(IMediator _mediator, IConfiguration config)
    : Endpoint<EmptyRequest, AuthResponse>
{
  private readonly IConfiguration _config = config;
  public override void Configure()
  {
    Post("/auth/refresh");
    AllowAnonymous();
    Summary(s =>
        s.Summary = "Refresh access token using secure cookie");
  }

  public override async Task HandleAsync(
    EmptyRequest req,
    CancellationToken ct
    )
  {
    var refreshToken = HttpContext.Request.Cookies["refreshToken"];
    Log.Information("Refresh token cookie value: {RefreshToken}", refreshToken ?? "null");
    
    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
    var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

    if (string.IsNullOrEmpty(refreshToken))
    {
      Log.Warning("Refresh token is missing in the request cookies");
      var unauthorized = Result<AuthResponse>.Unauthorized("Refresh token is required.");

      await SendResultAsync(unauthorized.ToMinimalApiResult());
      return;
    }

    // Send command to handler
    var result = await _mediator.Send(new RefreshTokenCommand(
        Token: refreshToken,
        IpAddress: ipAddress,
        DeviceInfo: userAgent
    ), ct);

    if (!result.IsSuccess)
    {
      CookieHelpers.ClearRefreshCookie(HttpContext.Response);
      await SendResultAsync(result.ToMinimalApiResult());
      return;
    }


    // Set the new refresh token cookie
    var expiryDays = _config.GetValue<int>("Jwt:RefreshTokenExpiryDays");

    CookieHelpers.SetRefreshCookie(HttpContext.Response, result.Value.RefreshToken , expiryDays);


    await SendOkAsync(new AuthResponse(
        result.Value.AccessToken,
        result.Value.Email,
        result.Value.Roles));
    return;
      }
}
