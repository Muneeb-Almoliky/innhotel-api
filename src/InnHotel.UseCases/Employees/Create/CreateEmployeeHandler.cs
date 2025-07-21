using InnHotel.Core.AuthAggregate;
using InnHotel.Core.BranchAggregate;
using InnHotel.Core.BranchAggregate.Specifications;
using InnHotel.Core.EmployeeAggregate;
using Microsoft.AspNetCore.Identity;
//using InnHotel.Core.EmployeeAggregate.Specifications;

namespace InnHotel.UseCases.Employees.Create;

public class CreateEmployeeHandler(IRepository<Employee> _repository, IRepository<Branch> _branchRepo, UserManager<ApplicationUser> _userManager)
    : ICommandHandler<CreateEmployeeCommand, Result<EmployeeDTO>>
{
    public async Task<Result<EmployeeDTO>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {

    var spec = new BranchByIdSpec(request.BranchId);
    if (!await _branchRepo.AnyAsync(spec, cancellationToken))
      return Result.Error($"Branch with Id {request.BranchId} not found.");

    

    if (!string.IsNullOrWhiteSpace(request.UserId))
    {
      var user = await _userManager.FindByIdAsync(request.UserId!);
      if (user == null)
        return Result.Error($"User with Id {request.UserId} not found.");
    }
    var employee = new Employee(
            branchId: request.BranchId,
            firstName: request.FirstName,
            lastName: request.LastName,
            hireDate: request.HireDate,
            position: request.Position,
            userId: request.UserId
        );

        await _repository.AddAsync(employee, cancellationToken);

        return new EmployeeDTO(
            employee.Id,
            employee.BranchId,
            employee.FirstName,
            employee.LastName,
            employee.HireDate,
            employee.Position,
            employee.UserId
        );
    }
}
