using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing Address entities in the database.
/// </summary>
public class AddressRepository : Repository<Address>, IAddressRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AddressRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Creates a new address in the database.
    /// </summary>
    /// <param name="address">The address entity to create.</param>
    /// <returns>The created address entity.</returns>
    public async Task<Address> CreateAddressAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
        return address;
    }
}
