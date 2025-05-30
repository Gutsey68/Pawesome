namespace Pawesome.Models.Dtos.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AdvertId { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}