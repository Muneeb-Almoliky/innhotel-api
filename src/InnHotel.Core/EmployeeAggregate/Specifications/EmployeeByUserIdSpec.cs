using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnHotel.Core.EmployeeAggregate.Specifications;
public sealed class EmployeeByUserIdSpec: Specification<Employee>
{
  public EmployeeByUserIdSpec(string userId) =>
      Query
          .Where(employee => employee.UserId == userId);
}
