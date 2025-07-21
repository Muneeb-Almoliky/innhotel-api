namespace InnHotel.Core.Services;
using InnHotel.Core.EmployeeAggregate.Events;
using InnHotel.Core.EmployeeAggregate;
using InnHotel.Core.Interfaces;

/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name="_repository"></param>
/// <param name="_mediator"></param>
/// <param name="_logger"></param>
public class DeleteEmployeeService(IRepository<Employee> _repository,
  IMediator _mediator,
  ILogger<DeleteEmployeeService> _logger) : IDeleteEmployeeService
{
  public async Task<Result> DeleteEmployee(int employeeId)
  {
    _logger.LogInformation("Deleting Employee {employeeId}", employeeId);
    Employee? aggregateToDelete = await _repository.GetByIdAsync(employeeId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new EmployeeDeletedEvent(employeeId);
    await _mediator.Publish(domainEvent);

    return Result.Success();
  }
}