using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels;

namespace Pawesome.Services;

/// <summary>
/// Service handling business logic for user profile operations
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the ProfileService
    /// </summary>
    /// <param name="userRepository">Repository for user operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public ProfileService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a user's profile information including address and pets
    /// </summary>
    /// <param name="userId">The ID of the user whose profile to retrieve</param>
    /// <returns>Profile view model if found, null otherwise</returns>
    public async Task<ProfileViewModel?> GetUserProfileAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

        if (user == null)
        {
            return null;
        }

        return _mapper.Map<ProfileViewModel>(user);
    }
}