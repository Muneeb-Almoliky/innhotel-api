namespace InnHotel.Core.EmployeeAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a employee is deleted.
/// The DeleteEmployeeService is used to dispatch this event.
/// </summary>
internal sealed class EmployeeDeletedEvent(int employeeId) : DomainEventBase
{
  public int employeeId { get; init; } = employeeId;
}