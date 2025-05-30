namespace Pawesome.Models.Dtos.Advert;

public class AdvertDto
{
    public int Id { get; set; }
    public required string OwnerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required string Status { get; set; }
    public decimal Amount { get; set; }
    public required string Type { get; set; }
    public required string City { get; set; }
    public DateTime CreatedAt { get; set; }
}