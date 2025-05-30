namespace Pawesome.Models.DTOs;

public class UserSimpleDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Photo { get; set; }
    public float? Rating { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAt { get; set; }
}