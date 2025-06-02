using Pawesome.Models.DTOs;
using Pawesome.Models.DTOs.Address;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels;

namespace Pawesome.Models.Dtos.Advert;

public class PetSittingAdvertDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AdvertStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string? AdditionalInformation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPetSitter { get; set; }
    public UserSimpleDto Owner { get; set; } = null!;
    public List<PetSimpleDto> Pets { get; set; } = [];
    public AddressDto? Address { get; set; }
    public string? City { get; set; }
    public List<Entities.AnimalType> AnimalTypes { get; set; } = [];
    
}
