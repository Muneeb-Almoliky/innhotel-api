namespace InnHotel.Web.Branches;

public class GetBranchByIdRequest
{
    public const string Route = "/Branches/{BranchId:int}";
    public static string BuildRoute(int branchId) => Route.Replace("{BranchId:int}", branchId.ToString());

    public int BranchId { get; set; }
}