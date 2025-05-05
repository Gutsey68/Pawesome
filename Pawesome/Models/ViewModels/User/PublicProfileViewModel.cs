namespace Pawesome.Models.ViewModels.User;

public class PublicProfileViewModel
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? Photo { get; set; }
    public string? Bio { get; set; }
    public float? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<PetViewModel>? Pets { get; set; }
    public bool IsCurrentUser { get; set; }
}