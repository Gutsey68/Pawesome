using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Services;

/// <summary>
/// Service handling business logic for user profile operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the UserService
    /// </summary>
    /// <param name="userRepository">Repository for user operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    /// <param name="environment">Web host environment for file operations</param>
    public UserService(IUserRepository userRepository, IMapper mapper, IWebHostEnvironment environment)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _environment = environment;
    }

    /// <summary>
    /// Retrieves user data for editing
    /// </summary>
    /// <param name="userId">The ID of the user to edit</param>
    /// <returns>Update the user view model if found, null otherwise</returns>
    public async Task<UpdateUserViewModel?> GetUserForEditAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return user == null ? null : _mapper.Map<UpdateUserViewModel>(user);
    }

    /// <summary>
    /// Retrieves a user's profile information including address and pets
    /// </summary>
    /// <param name="userId">The ID of the user whose profile to retrieve</param>
    /// <returns>Profile view model if found, null otherwise</returns>
    public async Task<ProfileViewModel?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

        return user == null ? null : _mapper.Map<ProfileViewModel>(user);
    }

    /// <summary>
    /// Updates user information based on the provided model
    /// </summary>
    /// <param name="model">The model containing updated user information</param>
    public async Task UpdateUserAsync(UpdateUserViewModel model)
    {
        var user = await _userRepository.GetByIdAsync(model.Id);

        if (user != null)
        {
            var oldPhoto = user.Photo;

            _mapper.Map(model, user);

            if (model.Photo is { Length: > 0 })
            {
                user.Photo = await SavePhotoAsync(model.Photo);

                if (!string.IsNullOrEmpty(oldPhoto))
                {
                    DeletePhoto(oldPhoto);
                }
            }
            else
            {
                user.Photo = oldPhoto;
            }

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Saves the uploaded photo to the file system
    /// </summary>
    /// <param name="photo">The photo file to save</param>
    /// <returns>The filename of the saved photo</returns>
    private async Task<string> SavePhotoAsync(IFormFile photo)
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
        var filePath = Path.Combine(_environment.WebRootPath, "images", "users", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await photo.CopyToAsync(stream);

        return fileName;
    }

    /// <summary>
    /// Deletes a photo from the file system
    /// </summary>
    /// <param name="fileName">The filename of the photo to delete</param>
    private void DeletePhoto(string fileName)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "images", "users", fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    /// <summary>
    /// Retrieves a public user profile for display
    /// </summary>
    /// <param name="userId">The ID of the user whose public profile to retrieve</param>
    /// <param name="currentUserId">The ID of the current user</param>
    /// <returns>Public profile view model if found, null otherwise</returns>
    public async Task<PublicProfileViewModel?> GetPublicUserProfileAsync(int userId, int currentUserId)
    {
        var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

        if (user == null)
        {
            return null;
        }

        var profile = new PublicProfileViewModel
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Photo = user.Photo,
            Bio = user.Bio,
            Rating = user.Rating,
            CreatedAt = user.CreatedAt,
            Pets = _mapper.Map<IEnumerable<PetViewModel>>(user.Pets),
            IsCurrentUser = userId == currentUserId
        };

        return profile;
    }
}