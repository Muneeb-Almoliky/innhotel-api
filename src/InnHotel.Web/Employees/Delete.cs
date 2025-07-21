using InnHotel.UseCases.Employees.Delete;
using InnHotel.Web.Common;

namespace InnHotel.Web.Employees;

/// <summary>
/// Delete a Employee.
/// </summary>
/// <remarks>
/// Delete a Employee by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<DeleteEmployeeRequest>
{
  public override void Configure()
  {
    Delete(DeleteEmployeeRequest.Route);
  }

  public override async Task HandleAsync(
    DeleteEmployeeRequest request,
    CancellationToken cancellationToken)
  {
    var command = new DeleteEmployeeCommand(request.EmployeeId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      var error = new FailureResponse(404, $"Employee with ID {request.EmployeeId} not found");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendAsync(new { status = 200, message = $"Employee with ID {request.EmployeeId} was successfully deleted" }, 
        statusCode: 200, 
        cancellation: cancellationToken);
      return;
    }

    await SendAsync(new FailureResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
  }
}