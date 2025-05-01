using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Interfaces;

public interface IUserService
{
    Task<ProfileViewModel?> GetUserProfileAsync(int userId);
    Task<UpdateUserViewModel?> GetUserForEditAsync(int id);
    Task UpdateUserAsync(UpdateUserViewModel model);
}