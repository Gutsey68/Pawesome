using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IPetRepository : IRepository<Pet>
{
    Task<IEnumerable<Pet>> GetPetsByUserIdAsync(int userId);
    Task<Pet?> GetPetWithDetailsAsync(int id);
}