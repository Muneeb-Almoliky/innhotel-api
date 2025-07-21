using InnHotel.Core.EmployeeAggregate;
using InnHotel.Core.EmployeeAggregate.Specifications;

namespace InnHotel.UseCases.Employees.Get;

public class GetEmployeeHandler(IReadRepository<Employee> _repository)
    : IQueryHandler<GetEmployeeQuery, Result<EmployeeDTO>>
{
    public async Task<Result<EmployeeDTO>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var spec = new EmployeeByIdSpec(request.EmployeeId);
        var employee = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

        if (employee == null)
            return Result.NotFound($"Employee with Id {request.EmployeeId} not found.");

        var employeeDto = new EmployeeDTO(
            employee.Id,
            employee.BranchId,
            employee.FirstName,
            employee.LastName,
            employee.HireDate,
            employee.Position,
            employee.UserId);

        return employeeDto;
    }
}
