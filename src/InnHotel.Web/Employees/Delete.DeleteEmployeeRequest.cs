namespace InnHotel.Web.Employees;

public record DeleteEmployeeRequest
{
  public const string Route = "Employees/{EmployeeId:int}";
  public static string BuildRoute(int employeeId) => Route.Replace("{EmployeeId:int}", employeeId.ToString());

  public int EmployeeId { get; set; }
}
