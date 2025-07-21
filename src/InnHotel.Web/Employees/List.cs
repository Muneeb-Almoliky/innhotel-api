using InnHotel.UseCases.Employees.List;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;
namespace InnHotel.Web.Employees;

/// <summary>
/// List all Employees with pagination support.
/// </summary>
/// <remarks>
/// Returns a paginated list of Employee records.
/// </remarks>
public class List(IMediator _mediator)
    : Endpoint<PaginationRequest, object>
{
    public override void Configure()
    {
        Get(ListEmployeeRequest.Route);
        Roles(AuthRoles.Admin.ToString(), AuthRoles.SuperAdmin.ToString(), AuthRoles.Receptionist.ToString());
        Summary(s =>
        {
          s.Summary = "List employees (paged)";
          s.Description = "Returns a paged list of employees.";
        });
  }

    public override async Task HandleAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var query = new ListEmployeeQuery(request.PageNumber, request.PageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            var (items, totalCount) = result.Value;
            var employeeRecords = items.Select(e => 
                new EmployeeRecord(
                    Id: e.Id,
                    BranchId: e.BranchId,
                    FirstName: e.FirstName,
                    LastName: e.LastName,
                    HireDate: e.HireDate,
                    Position: e.Position,
                    UserId: e.UserId
                )).ToList();

            var response = new PagedResponse<EmployeeRecord>(
                employeeRecords, 
                totalCount, 
                request.PageNumber, 
                request.PageSize);

            await SendOkAsync(response, cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }
}
