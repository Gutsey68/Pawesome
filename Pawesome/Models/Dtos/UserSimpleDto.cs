namespace Pawesome.Models.DTOs;

public class UserSimpleDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? Photo { get; set; }
    public float? Rating { get; set; }
}