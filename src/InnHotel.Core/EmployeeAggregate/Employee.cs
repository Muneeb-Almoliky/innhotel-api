using InnHotel.Core.AuthAggregate;
using InnHotel.Core.BranchAggregate;

namespace InnHotel.Core.EmployeeAggregate;

public class Employee(
    int branchId,
    string firstName,
    string lastName,
    DateOnly hireDate,
    string position,
    string? userId
) : EntityBase, IAggregateRoot
{
    public int BranchId { get; private set; } = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
    public Branch Branch { get; set; } = null!; 
    public string FirstName { get; set; } = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    public string LastName { get; set; } = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    public DateOnly HireDate { get; set; } = hireDate;
    public string Position { get; set; } = Guard.Against.NullOrEmpty(position, nameof(position));
    public string? UserId { get; private set; } = userId;
    public ApplicationUser? User { get; set; }

    public void UpdateDetails(
          int branchId,
          string firstName,
          string lastName,
          DateOnly hireDate,
          string position)
    {
      BranchId = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
      FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
      LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
      if (hireDate > DateOnly.FromDateTime(DateTime.UtcNow))
        throw new ArgumentException("Hire date cannot be in the future.", nameof(hireDate));
      HireDate = hireDate;
      Position = Guard.Against.NullOrEmpty(position, nameof(position));
    }

    /// <summary>Link or unlink this employee to an ApplicationUser account.</summary>
    public void AssignUser(string? userId)
    {
      UserId = userId;
    }
}
