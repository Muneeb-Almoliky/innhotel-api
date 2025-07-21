namespace InnHotel.Web.Rooms;

public class UpdateRoomStatusRequest
{
    public const string Route = "Rooms/{RoomId:int}/status";
    public static string BuildRoute(int roomId) => Route.Replace("{RoomId:int}", roomId.ToString());

    public int RoomId { get; set; }
    public int Status { get; set; }
} 
