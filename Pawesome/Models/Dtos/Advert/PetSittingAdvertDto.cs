using Pawesome.Models.DTOs;

namespace Pawesome.Models.Dtos.Advert;

public class PetSittingAdvertDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "pending";
    public decimal Amount { get; set; }
    public string? AdditionalInformation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPetSitter { get; set; }
    public UserSimpleDto? Owner { get; set; }
    public ICollection<PetSimpleDto> Pets { get; set; } = new List<PetSimpleDto>();
}