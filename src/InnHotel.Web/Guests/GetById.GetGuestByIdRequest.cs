namespace InnHotel.Web.Guests;

public class GetGuestByIdRequest
{
  public const string Route = "/Guests/{GuestId:int}";
  public static string BuildRoute(int guestId) => Route.Replace("{GuestId:int}", guestId.ToString());

  public int GuestId { get; set; }
}
