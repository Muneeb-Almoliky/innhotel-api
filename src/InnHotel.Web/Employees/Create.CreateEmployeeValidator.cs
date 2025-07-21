using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.Employees;

public class CreateEmployeeValidator : Validator<CreateEmployeeRequest>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.BranchId)
          .GreaterThan(0).WithMessage("BranchId must be a positive integer.");

        RuleFor(x => x.FirstName)
          .NotEmpty().MaximumLength(50);

        RuleFor(x => x.LastName)
          .NotEmpty().MaximumLength(50);

        RuleFor(x => x.HireDate)
          .NotNull().WithMessage("Hire date is required.")
            .Must(hd => hd.HasValue && hd.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
          .WithMessage("Hire date cannot be in the future.");

        RuleFor(x => x.Position)
          .NotEmpty().MaximumLength(50);
        
    }
}
