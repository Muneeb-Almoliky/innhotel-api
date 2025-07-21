namespace InnHotel.UseCases.Employees.Delete;
using InnHotel.Core.Interfaces;

public class DeleteEmployeeHandler(IDeleteEmployeeService _deleteEmployeeService)
  : ICommandHandler<DeleteEmployeeCommand, Result>
{
  public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken) =>
    await _deleteEmployeeService.DeleteEmployee(request.EmployeeId);
}