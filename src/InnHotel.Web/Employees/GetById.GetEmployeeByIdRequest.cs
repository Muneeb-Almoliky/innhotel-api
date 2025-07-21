namespace InnHotel.Web.Employees;

public class GetEmployeeByIdRequest
{
    public const string Route = "Employees/{id:int}";
    public static string BuildRoute(int id) => Route.Replace("{id:int}", id.ToString());
    [BindFrom("id")]
    public int EmployeeId { get; set; }
}
