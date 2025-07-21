using System.Globalization;
using InnHotel.UseCases.Employees.Create;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Employees;

/// <summary>
/// Create a new Employee.
/// </summary>
/// <remarks>
/// Creates a new Employee with the provided details.
/// </remarks>
public class Create(IMediator _mediator)
    : Endpoint<CreateEmployeeRequest, object>
{
    public override void Configure()
    {
        Post(CreateEmployeeRequest.Route);
        Roles(AuthRoles.Admin.ToString(), AuthRoles.SuperAdmin.ToString());
        Summary(s =>
        {
          s.Summary = "Create a new Employee record.";
          s.Description = "Creates an employee in a given branch; optionally links to an existing user account.";
          s.ExampleRequest = new CreateEmployeeRequest
          {
            BranchId = 1,
            FirstName = "Alice",
            LastName = "Johnson",
            HireDate = DateOnly.ParseExact("2024-05-01", "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Position = "Receptionist",
            UserId = null
          };
        });
  }

    public override async Task HandleAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEmployeeCommand(
            request.BranchId,
            request.FirstName!,
            request.LastName!,
            request.HireDate!.Value,
            request.Position!,
            request.UserId
        );

        var result = await _mediator.Send(command, cancellationToken);

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
                new { status = 201, message = "Employee created successfully", data = employeeRecord },
                statusCode: 201,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
}
