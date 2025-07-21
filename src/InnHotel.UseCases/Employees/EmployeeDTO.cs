namespace InnHotel.UseCases.Employees;

public record EmployeeDTO(
    int Id,
    int BranchId,
    string FirstName, 
    string LastName,
    DateOnly HireDate,
    string Position,
    string? UserId
) : ICommand<Result<EmployeeDTO>>;
