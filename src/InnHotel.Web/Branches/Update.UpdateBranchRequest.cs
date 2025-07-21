namespace InnHotel.Web.Branches;

public class UpdateBranchRequest
{
    public const string Route = "/Branches/{BranchId:int}";
    public static string BuildRoute(int branchId) => Route.Replace("{BranchId:int}", branchId.ToString());

    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}