namespace InnHotel.Web.Rooms;

public class UpdateRoomRequest
{
    public const string Route = "/Rooms/{RoomId:int}";
    public static string BuildRoute(int roomId) => Route.Replace("{RoomId:int}", roomId.ToString());

    public int RoomId { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Floor { get; set; }
}