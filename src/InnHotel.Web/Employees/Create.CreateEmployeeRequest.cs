using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.Employees;

public class CreateEmployeeRequest
{
    public const string Route = "Employees";

    [Required]
    public int BranchId { get; set; }

    [Required, MaxLength(50)]
    public string? FirstName { get; set; }

    [Required, MaxLength(50)]
    public string? LastName { get; set; }

    [Required]
    public DateOnly? HireDate { get; set; }

    [Required, MaxLength(50)]
    public string? Position { get; set; }

    /// <summary>
    /// Optional: link to an existing User account (null = no login)
    /// </summary>
    public string? UserId { get; set; }
}
