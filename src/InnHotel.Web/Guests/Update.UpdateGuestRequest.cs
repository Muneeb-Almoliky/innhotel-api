namespace InnHotel.Web.Guests;

public class UpdateGuestRequest
{
    public const string Route = "/Guests/{GuestId:int}";
    public static string BuildRoute(int guestId) => Route.Replace("{GuestId:int}", guestId.ToString());

  public int GuestId { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;

  public string Gender { get; set; } = string.Empty; // ✅ مضافة حديثًا لدعم التحويل لـ Enum

  public string IdProofType { get; set; } = string.Empty;
  public string IdProofNumber { get; set; } = string.Empty;
  public string? Email { get; set; }
  public string? Phone { get; set; }
  public string? Address { get; set; }
}
