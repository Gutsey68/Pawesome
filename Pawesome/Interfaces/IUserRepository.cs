using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserByIdWithDetailsAsync(int userId);
    public Task<bool> UpdateUserBalanceAsync(int userId, decimal amount);
    public Task<bool> SetStripeOnboardingCompletedAsync(int userId);
    Task<bool> UpdateUserBalanceToExactAmountAsync(int userId, decimal amount);

}