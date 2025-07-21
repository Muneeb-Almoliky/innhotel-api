namespace InnHotel.Web.Branches;

public class CreateBranchRequest
{
    public const string Route = "/Branches";

    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}