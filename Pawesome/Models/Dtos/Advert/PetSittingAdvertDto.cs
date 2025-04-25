using Pawesome.Models.DTOs;

namespace Pawesome.Models.Dtos.Advert;

public class PetSittingAdvertDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserSimpleDto Owner { get; set; } = null!;
    public List<PetSimpleDto> Pets { get; set; } = new();
    public bool IsPetSitter { get; set; }
    public List<AnimalTypeDto>? AcceptedAnimalTypes { get; set; }
}