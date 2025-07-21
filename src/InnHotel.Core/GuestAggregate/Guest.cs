namespace InnHotel.Core.GuestAggregate;
using Ardalis.GuardClauses;
using InnHotel.Core.GuestAggregate.ValueObjects;

public class Guest : EntityBase, IAggregateRoot
{
  // ========================
  // properties
  // ========================
  public string FirstName { get; private set; }
  public string LastName { get; private set; }
  public Gender Gender { get; private set; }
  public IdProofType IdProofType { get; private set; }
  public string IdProofNumber { get; private set; }
  public string? Email { get; private set; }
  public string? Phone { get; private set; }
  public string? Address { get; private set; }

  // ========================
  // constructor
  // ========================
  public Guest(
    string firstName,
    string lastName,
    Gender gender,
    IdProofType idProofType,
    string idProofNumber,
    string? email = null,
    string? phone = null,
    string? address = null
)
  {
    FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    Gender = gender;
    IdProofType = idProofType;
    IdProofNumber = Guard.Against.NullOrEmpty(idProofNumber, nameof(idProofNumber));
    Email = email;
    Phone = phone;
    Address = address;
  }

  // ========================
  // update method
  // ========================
  public void UpdateDetails(
      string firstName,
      string lastName,
      Gender gender,
      IdProofType idProofType,
      string idProofNumber,
      string? email = null,
      string? phone = null,
      string? address = null
  )
  {
    FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    Gender = gender;
    IdProofType = idProofType;
    IdProofNumber = Guard.Against.NullOrEmpty(idProofNumber, nameof(idProofNumber));
    Email = email;
    Phone = phone;
    Address = address;
  }
}
