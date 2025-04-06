using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.ViewModels;

namespace Pawesome.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProfileService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

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