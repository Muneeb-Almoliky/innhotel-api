using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.Auth;

public class LogoutRequest
{
    public const string Route = "/auth/logout";

    [Required]
    public string? RefreshToken { get; set; }
}