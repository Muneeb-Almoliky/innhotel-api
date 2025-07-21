namespace InnHotel.UseCases.Employees.Create;

public record CreateEmployeeCommand(
    int BranchId,
    string FirstName,
    string LastName,
    DateOnly HireDate,
    string Position,
    string? UserId
) : ICommand<Result<EmployeeDTO>>;
