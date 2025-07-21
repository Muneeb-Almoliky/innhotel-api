using FastEndpoints;
using InnHotel.UseCases.Employees.Get;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Employees;

/// <summary>
/// Get a Employee by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Employee record.
/// </remarks>
public class GetById(IMediator _mediator)
    : Endpoint<GetEmployeeByIdRequest, object>
{
    public override void Configure()
    {
        Get(GetEmployeeByIdRequest.Route);
        Roles(AuthRoles.Admin, AuthRoles.SuperAdmin);
        Description(d => d
            .Produces<EmployeeRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(GetEmployeeByIdRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeeQuery(request.EmployeeId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            var error = new FailureResponse(404, $"Employee with ID {request.EmployeeId} not found");
            await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            var employee = result.Value;
            Response = new EmployeeRecord(
                  employee.Id,
                  employee.BranchId, 
                  employee.FirstName, 
                  employee.LastName, 
                  employee.HireDate, 
                  employee.Position, 
                  employee.UserId);
            await SendOkAsync(Response, cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }
}
