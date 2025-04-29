using Pawesome.Models.ViewModels;

namespace Pawesome.Interfaces;

using Pawesome.Models.DTOs;

public interface IProfileService
{
    Task<ProfileViewModel?> GetUserProfileAsync(int userId);
    Task<UpdateUserDto?> GetUserForEditAsync(int id);
    Task UpdateUserAsync(UpdateUserDto userDto);
}