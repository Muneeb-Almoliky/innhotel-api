using FastEndpoints;
using FluentValidation;
using global::InnHotel.Web.Employees;

namespace InnHotel.Web.Employees;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeleteEmployeeValidator : Validator<DeleteEmployeeRequest>
{
  public DeleteEmployeeValidator()
  {
     RuleFor(x => x.EmployeeId)
      .GreaterThan(0).WithMessage("Employee Id must be a positive integer.");
  }
}
