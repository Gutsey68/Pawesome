namespace Pawesome.Models.ViewModels.Dashboard;

public class DashboardViewModel
{
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(1);
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(2);
}