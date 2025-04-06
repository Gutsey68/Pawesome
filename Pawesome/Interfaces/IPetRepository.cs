using Pawesome.Models;

namespace Pawesome.Interfaces;

public interface IPetRepository : IRepository<Pet>
{
    Task<IEnumerable<Pet>> GetPetsByUserIdAsync(int userId);
    Task<Pet?> GetPetWithDetailsAsync(int id);
}