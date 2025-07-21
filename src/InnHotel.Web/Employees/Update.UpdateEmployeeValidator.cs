using FluentValidation;

namespace InnHotel.Web.Employees;

public class UpdateEmployeeValidator : Validator<UpdateEmployeeRequest>
{
  public UpdateEmployeeValidator()
  {
    RuleFor(x => x.BranchId).GreaterThan(0);
    RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2);
    RuleFor(x => x.LastName).NotEmpty().MinimumLength(2);
    RuleFor(x => x.HireDate)
        .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
        .WithMessage("Hire date cannot be in the future.");
    RuleFor(x => x.Position).NotEmpty();
    RuleFor(x => x.UserId)
        .Must(userId => userId == null || !string.IsNullOrWhiteSpace(userId))
        .WithMessage("UserId must be non-empty if provided.");
  }
}
