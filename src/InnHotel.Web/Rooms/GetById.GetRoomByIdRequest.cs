using FastEndpoints;

namespace InnHotel.Web.Rooms;

public class GetRoomByIdRequest
{
    public const string Route = "/rooms/{id:int}";
    public static string BuildRoute(int id) => Route.Replace("{id:int}", id.ToString());

    [BindFrom("id")]
    public int RoomId { get; set; }
}