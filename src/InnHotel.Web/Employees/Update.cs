using InnHotel.Core.BranchAggregate;
using InnHotel.UseCases.Employees.Update;
using InnHotel.Web.Common;
using Sprache;

namespace InnHotel.Web.Employees;

/// <summary>
/// Update an existing Employee.
/// </summary>
/// <remarks>
/// Update an existing Employee by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<UpdateEmployeeRequest, object>
{
    public override void Configure()
    {
        Put(UpdateEmployeeRequest.Route);
    }

    public override async Task HandleAsync(
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmployeeCommand(
            request.EmployeeId,
            request.BranchId,
            request.FirstName!,
            request.LastName!,
            request.HireDate!.Value,
            request.Position!,
            request.UserId

        );

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendAsync(
                new FailureResponse(404, $"Employee with ID {request.EmployeeId} not found"),
                statusCode: 404,
                cancellation: cancellationToken);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendAsync(
                new FailureResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            var employeeRecord = new EmployeeRecord(
                result.Value.Id, 
                result.Value.BranchId, 
                result.Value.FirstName,
                result.Value.LastName, 
                result.Value.HireDate, 
                result.Value.Position,
                result.Value.UserId
            );

            await SendAsync(
                new { status = 200, message = "Employee updated successfully", data = employeeRecord },
                statusCode: 200,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
}
