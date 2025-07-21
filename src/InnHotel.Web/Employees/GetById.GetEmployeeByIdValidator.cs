using InnHotel.Web.Rooms;
using FluentValidation;

namespace InnHotel.Web.Employees;

public class GetEmployeeByIdValidator : Validator<GetEmployeeByIdRequest>
{
  public GetEmployeeByIdValidator()
  {
    RuleFor(x => x.EmployeeId)
        .GreaterThan(0).WithMessage("Room ID must be greater than 0");
  }
}
