namespace InnHotel.Web.Reservations;

public class CreateReservationRequest
{
  public const string Route = "reservations";

  public int GuestId { get; set; }
  public DateTime CheckInDate { get; set; }
  public DateTime CheckOutDate { get; set; }
  public List<ReservationRoomRequest> Rooms { get; set; } = new();
  public List<ReservationServiceRequest> Services { get; set; } = new();
}
