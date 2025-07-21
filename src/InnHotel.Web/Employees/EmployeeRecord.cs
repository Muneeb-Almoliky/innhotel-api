namespace InnHotel.Web.Employees;

public record EmployeeRecord(
    int Id,
    int BranchId,
    string FirstName,
    string LastName,
    DateOnly HireDate,
    string Position,
    string? UserId
);
