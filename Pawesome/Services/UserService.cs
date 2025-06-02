using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Pawesome.Interfaces;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Pet;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Services;

/// <summary>
/// Service handling business logic for user profile operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<User> _userManager;
    private readonly ICityRepository _cityRepository;
    private readonly IAdvertRepository _advertRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IAddressRepository _addressRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="userRepository">Repository for user operations.</param>
    /// <param name="mapper">AutoMapper instance for object mapping.</param>
    /// <param name="environment">Web host environment for file operations.</param>
    /// <param name="cityRepository">Repository for city operations.</param>
    /// <param name="countryRepository">Repository for country operations.</param>
    /// <param name="addressRepository">Repository for address operations.</param>
    /// <param name="advertRepository"></param>
    /// <param name="userManager">User manager for user-related operations.</param>
    public UserService(
        IUserRepository userRepository, 
        IMapper mapper, 
        IWebHostEnvironment environment, 
        ICityRepository cityRepository,
        ICountryRepository countryRepository,
        IAddressRepository addressRepository,
        IAdvertRepository advertRepository,
        UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _environment = environment;
        _userManager = userManager;
        _cityRepository = cityRepository;
        _countryRepository = countryRepository;
        _advertRepository = advertRepository;
        _addressRepository = addressRepository;
    }

    /// <summary>
    /// Retrieves user data for editing.
    /// </summary>
    /// <param name="userId">The ID of the user to edit.</param>
    /// <returns>Update the user view model if found, null otherwise.</returns>
    public async Task<UpdateUserViewModel?> GetUserForEditAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return user == null ? null : _mapper.Map<UpdateUserViewModel>(user);
    }

    /// <summary>
    /// Retrieves a user's profile information including address and pets.
    /// </summary>
    /// <param name="userId">The ID of the user whose profile to retrieve.</param>
    /// <returns>Profile view model if found, null otherwise.</returns>
    public async Task<ProfileViewModel?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
    
        if (user == null)
            return null;
    
        var profileViewModel = _mapper.Map<ProfileViewModel>(user);
    
        return profileViewModel;
    }

    /// <summary>
    /// Updates user information based on the provided model.
    /// </summary>
    /// <param name="model">The model containing updated user information.</param>
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

            await UpdateUserAddressAsync(user, model);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            
            await UpdateUserClaimsAsync(user);
        }
    }

    /// <summary>
    /// Saves the uploaded photo to the file system.
    /// </summary>
    /// <param name="photo">The photo file to save.</param>
    /// <returns>The filename of the saved photo.</returns>
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
    /// Deletes a photo from the file system.
    /// </summary>
    /// <param name="fileName">The filename of the photo to delete.</param>
    private void DeletePhoto(string fileName)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "images", "users", fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    /// <summary>
    /// Retrieves a public user profile for display.
    /// </summary>
    /// <param name="userId">The ID of the user whose public profile to retrieve.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <returns>Public profile view model if found, null otherwise.</returns>
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
            Adverts = _mapper.Map<List<PetSittingAdvertDto>>(user.Adverts),
            IsCurrentUser = userId == currentUserId
        };

        return profile;
    }
    
    /// <summary>
    /// Updates the user's claims in the identity system.
    /// Removes all existing claims and adds updated claims for first name, last name, email, and photo.
    /// </summary>
    /// <param name="user">The user whose claims to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task UpdateUserClaimsAsync(User user)
    {
        var existingClaims = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, existingClaims);
    
        var claims = new List<Claim>
        {
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
        };

        claims.Add(new Claim("Address", user.Address?.StreetAddress ?? string.Empty));

        if (user.Email != null) 
        {
            claims.Add(new Claim("Email", user.Email));
        }
    
        if (!string.IsNullOrEmpty(user.Photo))
        {
            claims.Add(new Claim("Photo", user.Photo));
        }
    
        await _userManager.AddClaimsAsync(user, claims);
    }
    
    /// <summary>
    /// Updates the user's address information.
    /// Ensures the country and city exist in the database, creates them if necessary,
    /// and updates the user's address with the provided street address, additional info, and city.
    /// </summary>
    /// <param name="user">The user whose address will be updated.</param>
    /// <param name="model">The view model containing the new address information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task UpdateUserAddressAsync(User user, UpdateUserViewModel model)
    {
        if (!string.IsNullOrEmpty(model.StreetAddress) && !string.IsNullOrEmpty(model.City))
        {
            var country = await _countryRepository.GetByNameAsync("France");

            if (country == null)
            {
                country = new Country {
                    Name = "France",
                    Cities = new List<City>()
                };
                await _countryRepository.AddAsync(country);
                await _countryRepository.SaveChangesAsync();
            }

            var city = await _cityRepository.GetByNameAndPostalCodeAsync(model.City, model.PostalCode ?? "00000");

            if (city == null)
            {
                city = new City
                {
                    Name = model.City,
                    PostalCode = model.PostalCode ?? "00000",
                    Country = country,
                    CountryId = country.Id,
                    Addresses = new List<Address>()
                };
                await _cityRepository.AddAsync(city);
                await _cityRepository.SaveChangesAsync();
            }

            if (user.Address != null)
            {
                user.Address.StreetAddress = model.StreetAddress;
                user.Address.AdditionalInfo = model.AdditionalInfo;
                user.Address.CityId = city.Id;
                user.Address.City = city;
                user.Address.UpdatedAt = DateTime.UtcNow;

                await _addressRepository.UpdateAsync(user.Address);
            }

            await _addressRepository.SaveChangesAsync();
        }
    }
    
    public int GetUsersCount() 
    {
        return _userRepository.GetAllAsync().Result.Count();
    }

    public async Task<List<UserSimpleDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = _mapper.Map<List<UserSimpleDto>>(users);
    
        foreach (var userDto in userDtos)
        {
            var user = users.First(u => u.Id == userDto.Id);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.FirstOrDefault() ?? "User";
        }
    
        return userDtos;
    }
    
    public async Task<bool> BanUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return false;
            
        user.Status = UserStatus.Banned;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> UnbanUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return false;
            
        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> RateUserAsync(int raterUserId, int ratedUserId, int rating, string? comment, int advertId)
    {
        try
        {
            var ratedUser = await _userRepository.GetByIdAsync(ratedUserId);
            var raterUser = await _userRepository.GetByIdAsync(raterUserId);
            var advert = await _advertRepository.GetAdvertByIdAsync(advertId);
        
            if (ratedUser == null || raterUser == null || advert == null)
            {
                return false;
            }
            
            if (advert.Status != AdvertStatus.FullyBooked)
            {
                return false;
            }
        
            var review = new Review
            {
                UserId = ratedUserId,
                Rate = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                User = ratedUser,
                Advert = advert
            };
        
            if (ratedUser.Reviews == null)
                ratedUser.Reviews = new List<Review>();
            
            ratedUser.Reviews.Add(review);
        
            var allRatings = ratedUser.Reviews.Select(r => r.Rate).ToList();
            if (allRatings.Any())
            {
                ratedUser.Rating = allRatings.Average();
            }
        
            await _userRepository.UpdateAsync(ratedUser);
            await _userRepository.SaveChangesAsync();
        
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la notation de l'utilisateur: {ex.Message}");
            return false;
        }
    }
}
