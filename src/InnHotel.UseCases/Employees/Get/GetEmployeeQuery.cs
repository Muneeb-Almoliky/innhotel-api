namespace InnHotel.UseCases.Employees.Get;

public record GetEmployeeQuery(int EmployeeId) : IQuery<Result<EmployeeDTO>>;