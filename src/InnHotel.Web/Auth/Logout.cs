using Ardalis.Result.AspNetCore;
using InnHotel.Core.AuthAggregate;
using InnHotel.UseCases.Auth.Logout;
using InnHotel.Web.Helpers;
using InnHotel.Web.Common;
using System.Security.Claims;

namespace InnHotel.Web.Auth;

public class Logout(IMediator _mediator) : EndpointWithoutRequest<object>
{
    public override void Configure()
    {
        Post(LogoutRequest.Route);
        Summary(s =>
        {
            s.Summary = "Logout a user";
            s.Description = "Invalidates the refresh token and clears the cookie";
            s.ExampleRequest = new LogoutRequest
            {
                RefreshToken = "your-refresh-token"
            };
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            await SendAsync(
                new FailureResponse(401, "Unauthorized."),
                statusCode: 401,
                cancellation: ct
            );
            return;
        }

        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            await SendAsync(
                new FailureResponse(400, "Missing refresh token cookie."),
                statusCode: 400,
                cancellation: ct
            );
            return;
        }

        var result = await _mediator.Send(
            new LogoutCommand(userId, refreshToken),
            ct
        );

        if (result.Status == ResultStatus.NotFound)
        {
            await SendAsync(
                new FailureResponse(404, result.Errors.First()),
                statusCode: 404,
                cancellation: ct
            );
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendAsync(
                new FailureResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: ct
            );
            return;
        }

        if (!result.IsSuccess)
        {
            await SendAsync(
                new FailureResponse(500, "Unexpected error."),
                statusCode: 500,
                cancellation: ct
            );
            return;
        }

        CookieHelpers.ClearRefreshCookie(HttpContext.Response);

        await SendAsync(
            new { status = 200, message = "Logged out successfully." },
            statusCode: 200,
            cancellation: ct
        );
    }
}