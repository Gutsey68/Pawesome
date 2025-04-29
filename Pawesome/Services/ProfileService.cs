using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels;
using Pawesome.Models.DTOs;

namespace Pawesome.Services;

/// <summary>
/// Service handling business logic for user profile operations
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the ProfileService
    /// </summary>
    /// <param name="userRepository">Repository for user operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public ProfileService(IUserRepository userRepository, IMapper mapper,IWebHostEnvironment environment)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _environment = environment;
    }

    /// <summary>
    /// Retrieves a user's profile information including address and pets
    /// </summary>
    /// <param name="userId">The ID of the user whose profile to retrieve</param>
    /// <returns>Profile view model if found, null otherwise</returns>
    public async Task<UpdateUserDto?> GetUserForEditAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        // Mapper directement vers UpdateUserDto
        return _mapper.Map<UpdateUserDto>(user);
    }

public async Task<ProfileViewModel?> GetUserProfileAsync(int userId)
{
    var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

    if (user == null)
    {
        return null;
    }

    return _mapper.Map<ProfileViewModel>(user);
}

    public async Task UpdateUserAsync(UpdateUserDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(userDto.Id);

        if (user != null)
        {
            var oldPhoto = user.Photo;

            _mapper.Map(userDto, user);

            if (userDto.Photo != null && userDto.Photo.Length > 0)
            {
                user.Photo = await SavePhotoAsync(userDto.Photo);

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
    
    private async Task<string> SavePhotoAsync(IFormFile photo)
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
        var filePath = Path.Combine(_environment.WebRootPath, "images", "users", fileName);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await photo.CopyToAsync(stream);

        return fileName;
    }
    
    private void DeletePhoto(string fileName)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "images", "users", fileName);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}