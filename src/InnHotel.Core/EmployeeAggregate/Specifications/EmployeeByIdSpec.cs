using Ardalis.Specification;

namespace InnHotel.Core.EmployeeAggregate.Specifications;

public sealed class EmployeeByIdSpec : Specification<Employee>
{
    public EmployeeByIdSpec(int employeeId)
    {
        Query
            .Where(employee => employee.Id == employeeId)
            .Include(employee => employee.Branch)
            .Include(employee => employee.User);
    }
}
