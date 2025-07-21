namespace InnHotel.UseCases.Employees.List;

public record ListEmployeeQuery(int PageNumber, int PageSize) 
    : IQuery<Result<(List<EmployeeDTO> Items, int TotalCount)>>;