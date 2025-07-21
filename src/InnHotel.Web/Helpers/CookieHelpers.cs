namespace InnHotel.Web.Helpers;

public static class CookieHelpers
{
  public static CookieOptions GetRefreshCookieOpts(int expiryDays)
  {
    return new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.None,
      Expires = DateTime.UtcNow.AddDays(expiryDays),
      Path = "/"
    };
  }

  public static void SetRefreshCookie(
      HttpResponse response,
      string token,
      int expiryDays)
  {
    response.Cookies.Append("refreshToken", token, GetRefreshCookieOpts(expiryDays));
  }

  public static void ClearRefreshCookie(HttpResponse response)
  {
    response.Cookies.Delete("refreshToken", GetRefreshCookieOpts(0));
  }
}
