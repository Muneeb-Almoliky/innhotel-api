namespace InnHotel.Web.Test;
using FastEndpoints;

public class TestEndpoint : Endpoint<EmptyRequest, TestResponse>
{
  public override void Configure()
  {
    Get("test");
    Description(b => b
        .Produces<TestResponse>(200)
        .ProducesProblemDetails(500));
    AllowAnonymous();
  }

  public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
  {
    var response = new TestResponse
    {
      Message = "API is working!",
      Timestamp = DateTime.UtcNow
    };

    await SendAsync(response);
  }
}

public class TestResponse
{
    public required string Message { get; set; } 
    public DateTime Timestamp { get; set; }
}
