namespace InnHotel.Core.AuthAggregate.Specifications;
public sealed class UserByIdSpec: Specification<ApplicationUser>
{
  public UserByIdSpec(string userId) =>  
    Query
      .Where(user => user.Id == userId);
  
}
