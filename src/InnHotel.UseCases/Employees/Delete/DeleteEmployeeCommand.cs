namespace InnHotel.UseCases.Employees.Delete;

public record DeleteEmployeeCommand(int EmployeeId) : ICommand<Result>;