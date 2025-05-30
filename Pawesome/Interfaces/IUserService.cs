using Pawesome.Models.DTOs;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Interfaces;

public interface IUserService
{
    Task<ProfileViewModel?> GetUserProfileAsync(int userId);
    Task<UpdateUserViewModel?> GetUserForEditAsync(int id);
    Task UpdateUserAsync(UpdateUserViewModel model);
    Task<PublicProfileViewModel?> GetPublicUserProfileAsync(int userId, int currentUserId);
    public int GetUsersCount();
    public Task<List<UserSimpleDto>> GetAllUsers();
    Task<bool> BanUserAsync(int userId);
    Task<bool> UnbanUserAsync(int userId);
}