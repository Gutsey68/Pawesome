

using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IAddressRepository : IRepository<Address>
{
    Task<Address> CreateAddressAsync(Address address);
}