namespace Pawesome.Interfaces;

public class IAdvertSortingOptions
{
    public string SortBy { get; set; } = "recent"; // Valeurs possibles : "recent", "oldest", "soon"
    public string SortDirection { get; set; } = "desc"; // Valeurs possibles : "asc", "desc"
}