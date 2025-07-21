namespace InnHotel.Web.Rooms;

public record DeleteRoomRequest
{
  public const string Route = "/Rooms/{RoomId:int}";
  public static string BuildRoute(int roomId) => Route.Replace("{RoomId:int}", roomId.ToString());

  public int RoomId { get; set; }
}