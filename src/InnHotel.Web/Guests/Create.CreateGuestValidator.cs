using Ardalis.SharedKernel;
using FluentValidation;
using InnHotel.Core.GuestAggregate.ValueObjects;

namespace InnHotel.Web.Guests;

/// <summary>
/// Validates incoming CreateGuestRequest
/// </summary>
public class CreateGuestValidator : Validator<CreateGuestRequest>
{
  public CreateGuestValidator()
  {
    RuleFor(x => x.FirstName)
        .NotEmpty().WithMessage("First name is required.")
        .MaximumLength(50);

    RuleFor(x => x.LastName)
        .NotEmpty().WithMessage("Last name is required.")
        .MaximumLength(50);

    RuleFor(x => x.Gender)
        .NotEmpty().WithMessage("Gender is required.")
        .Must(gender => Enum.TryParse<Gender>(gender, true, out _))
        .WithMessage("Invalid gender value.")
        .MaximumLength(20);

    RuleFor(x => x.IdProofType)
        .NotEmpty().WithMessage("ID proof type is required.")
        .Must(id => Enum.TryParse<IdProofType>(id, true, out _))
        .WithMessage("Invalid ID proof type value.")
        .MaximumLength(50);

    RuleFor(x => x.IdProofNumber)
        .NotEmpty().WithMessage("ID proof number is required.")
        .MaximumLength(50);

    RuleFor(x => x.Email)
        .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
        .MaximumLength(100);

    RuleFor(x => x.Phone)
        .Matches(@"^[\d\-\+\s]+$").When(x => !string.IsNullOrEmpty(x.Phone))
        .MaximumLength(20);

    RuleFor(x => x.Address)
        .MaximumLength(500);
  }
}
