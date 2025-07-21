using InnHotel.Core.EmployeeAggregate;

namespace InnHotel.UseCases.Employees.List;

public class ListEmployeeHandler(IReadRepository<Employee> _repository)
    : IQueryHandler<ListEmployeeQuery, Result<(List<EmployeeDTO> Items, int TotalCount)>>
{
    public async Task<Result<(List<EmployeeDTO> Items, int TotalCount)>> Handle(ListEmployeeQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _repository.CountAsync(cancellationToken);
        
        var employees = await _repository.ListAsync(cancellationToken);
        var pagedEmployees = employees
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var employeeDtos = pagedEmployees.Select(entity => new EmployeeDTO(
            entity.Id,
            entity.BranchId,
            entity.FirstName,
            entity.LastName,
            entity.HireDate,
            entity.Position,
            entity.UserId
            )).ToList();

        return (employeeDtos, totalCount);
    }
}
