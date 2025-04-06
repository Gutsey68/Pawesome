using Pawesome.Models.ViewModels;

namespace Pawesome.Interfaces;

public interface IProfileService
{
    Task<ProfileViewModel?> GetUserProfileAsync(string userId);
}