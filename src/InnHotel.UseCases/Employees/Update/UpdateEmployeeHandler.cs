using InnHotel.Core.AuthAggregate;
using InnHotel.Core.EmployeeAggregate;
using InnHotel.Core.EmployeeAggregate.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.UseCases.Employees.Update;

public class UpdateEmployeeHandler(IRepository<Employee> _repository, UserManager<ApplicationUser> _userManager)
    : ICommandHandler<UpdateEmployeeCommand, Result<EmployeeDTO>>
{
    public async Task<Result<EmployeeDTO>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var spec = new EmployeeByIdSpec(request.EmployeeId);
        var employee = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (employee == null)
            return Result.NotFound($"Employee {request.EmployeeId} not found.");
     
        employee.UpdateDetails(
            request.BranchId,
            request.FirstName,
            request.LastName,
            request.HireDate,
            request.Position);

        if(request.UserId != employee.UserId)
        {
          // Check if the user exists if a new user is being assigned
          if (!string.IsNullOrWhiteSpace(request.UserId))
          {
            // Assuming you have a UserManager or similar service to validate user existence
             var user = await _userManager.FindByIdAsync(request.UserId);
             if (user == null)
              return Result.Error($"User with Id {request.UserId} not found.");

            // Check if the user is already assigned to another employee
            var existingEmployeeSpec = new EmployeeByUserIdSpec(request.UserId);
            var existingEmployee = await _repository.FirstOrDefaultAsync(existingEmployeeSpec, cancellationToken);
            if (existingEmployee != null && existingEmployee.Id != request.EmployeeId)
              return Result.Error($"User with Id {request.UserId} is already assigned to another employee.");

            // Assign the user to the employee
            employee.AssignUser(request.UserId);
          }
        }

    try
        {
            await _repository.UpdateAsync(employee, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_") == true)
        {
            return Result.Error("A employee with these details already exists.");
        }

        return new EmployeeDTO(
            employee.Id, 
            employee.BranchId, 
            employee.FirstName,
            employee.LastName, 
            employee.HireDate, 
            employee.Position,
            employee.UserId);
    }
}
