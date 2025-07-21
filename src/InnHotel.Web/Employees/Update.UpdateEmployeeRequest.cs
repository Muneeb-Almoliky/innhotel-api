using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.Employees;

public class UpdateEmployeeRequest
{
    public const string Route = "Employees/{EmployeeId:int}";
    public static string BuildRoute(int employeeId) => Route.Replace("{EmployeeId:int}", employeeId.ToString());

    public int EmployeeId { get; set; }

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

    public string? UserId { get; set; }
}
