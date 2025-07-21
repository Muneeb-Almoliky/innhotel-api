using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Request DTO to create a new Room
/// </summary>
public class CreateRoomRequest
{
    public const string Route = "/rooms";

    [Required]
    public int BranchId { get; set; }

    [Required]
    public int RoomTypeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string? RoomNumber { get; set; }

    [Required]
    public RoomStatus Status { get; set; }

    [Required]
    [Range(0, 100)]
    public int Floor { get; set; }
}