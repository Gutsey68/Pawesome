using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Pet;
using Pawesome.Models.ViewModels.User;
using Pawesome.Services;

namespace Pawesome.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IWebHostEnvironment> _envMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ICityRepository> _cityRepoMock = new();
    private readonly Mock<ICountryRepository> _countryRepoMock = new();
    private readonly Mock<IAddressRepository> _addressRepoMock = new();
    private readonly Mock<IAdvertRepository> _advertRepoMock = new();

    private readonly UserService _userService;

    public UserServiceTests()
    {
        var store = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            store.Object,
            default!, default!, default!, default!,
            default!, default!, default!, default!
        );

        _userService = new UserService(
            _userRepoMock.Object,
            _mapperMock.Object,
            _envMock.Object,
            _cityRepoMock.Object,
            _countryRepoMock.Object,
            _addressRepoMock.Object,
            _advertRepoMock.Object,
            _userManagerMock.Object
        );
    }

    private User CreateGauthierUser(int userId) => new User
    {
        Id = userId,
        FirstName = "Gauthier",
        LastName = "Seyzeriat",
        Email = "gseyzeriat@example.com",
        UserName = "Gauthier",
        Pets = new List<Pet>(),
        Notifications = new List<Notification>(),
        Reports = new List<Report>(),
        PasswordResets = new List<PasswordReset>(),
        SentMessages = new List<Message>(),
        ReceivedMessages = new List<Message>(),
        Reviews = new List<Review>(),
        Adverts = new List<Advert>()
    };

    [Fact]
    public async Task GetUserForEditAsync_UserExists_ReturnsMappedViewModel()
    {
        // Arrange
        var userId = 1;
        var userEntity = CreateGauthierUser(userId);

        var expectedViewModel = new UpdateUserViewModel
        {
            Id = userId,
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            Photo = null,
            ExistingPhoto = "",
            PhoneNumber = "0123456789",
            City = "",
            StreetAddress = "",
            AdditionalInfo = "",
            PostalCode = ""
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userEntity);
        _mapperMock.Setup(m => m.Map<UpdateUserViewModel>(userEntity)).Returns(expectedViewModel);

        // Act
        var result = await _userService.GetUserForEditAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);
        Assert.Equal("Gauthier", result.FirstName);
    }

    [Fact]
    public async Task GetUserForEditAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        // Act
        var result = await _userService.GetUserForEditAsync(42);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_UserExists_ReturnsProfileViewModel()
    {
        // Arrange
        var userId = 2;
        var userEntity = CreateGauthierUser(userId);

        var expectedViewModel = new ProfileViewModel
        {
            Id = userId,
            Email = "gseyzeriat@example.com",
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            PhoneNumber = "0123456789",
            Bio = "Bio",
            Photo = "photo.jpg",
            Status = "Active",
            IsVerified = true,
            BalanceAccount = 100.00m,
            Rating = 4.5f,
            CreatedAt = DateTime.UtcNow,
            Street = "1 rue du test",
            City = "Lyon",
            PostalCode = "69000",
            Country = "France",
            Pets = new List<PetViewModel>()
        };

        _userRepoMock.Setup(r => r.GetUserByIdWithDetailsAsync(userId)).ReturnsAsync(userEntity);
        _mapperMock.Setup(m => m.Map<ProfileViewModel>(userEntity)).Returns(expectedViewModel);

        // Act
        var result = await _userService.GetUserProfileAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Gauthier", result!.FirstName);
        Assert.Equal("Seyzeriat", result.LastName);
        Assert.Equal("Lyon", result.City);
    }


    [Fact]
    public async Task GetUserProfileAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetUserByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        // Act
        var result = await _userService.GetUserProfileAsync(999);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetPublicUserProfileAsync_UserExists_ReturnsViewModel()
    {
        // Arrange
        var userId = 3;
        var currentUserId = 3;
        var user = CreateGauthierUser(userId);
        user.Photo = "photo.jpg";
        user.Bio = "Bio de Gauthier";
        user.Rating = 4.2f;
        user.CreatedAt = new DateTime(2020, 1, 1);
        user.Pets = new List<Pet>();

        var expectedPets = new List<PetViewModel>
        {
            new PetViewModel
            {
                Name = "Milo",
                Species = "Dog",
                Photo = "milo.jpg",
            }
        };

        _userRepoMock.Setup(r => r.GetUserByIdWithDetailsAsync(userId)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<IEnumerable<PetViewModel>>(user.Pets)).Returns(expectedPets);

        // Act
        var result = await _userService.GetPublicUserProfileAsync(userId, currentUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);
        Assert.Equal("Gauthier Seyzeriat", result.FullName);
        Assert.Equal("photo.jpg", result.Photo);
        Assert.Equal("Bio de Gauthier", result.Bio);
        Assert.Equal(4.2f, result.Rating);
        Assert.Equal(new DateTime(2020, 1, 1), result.CreatedAt);
        Assert.True(result.IsCurrentUser);
        if (result.Pets != null)
        {
            Assert.Single(result.Pets);
            Assert.Equal("Milo", result.Pets.First().Name);
        }
    }
    
    [Fact]
    public async Task GetPublicUserProfileAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetUserByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        // Act
        var result = await _userService.GetPublicUserProfileAsync(10, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_UserNotFound_DoesNothing()
    {
        // Arrange
        var model = new UpdateUserViewModel
        {
            Id = 99,
            LastName = "Seyzeriat",
            FirstName = "Gauthier",
            Bio = "Bio",
            Photo = new FormFile(Stream.Null, 0, 0, "Photo", "photo.jpg"),
            ExistingPhoto = "photo.jpg",
            PhoneNumber = "0123456789",
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync((User)null!);

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_UserExists_NoNewPhoto_KeepsOldPhoto()
    {
        // Arrange
        var user = CreateGauthierUser(1);
        user.Photo = "old-photo.jpg";

        var model = new UpdateUserViewModel
        {
            Id = 1,
            Photo = null,
            StreetAddress = "Rue X",
            City = "Ville",
            PostalCode = "12345",
            AdditionalInfo = "",
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            PhoneNumber = "0123456789",
            ExistingPhoto = "old-photo.jpg"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(user);

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        Assert.Equal("old-photo.jpg", user.Photo);
        _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_NewPhotoUploaded_SavesAndDeletesOld()
    {
        // Arrange
        var user = CreateGauthierUser(1);
        user.Photo = "old-photo.jpg";

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(123);
        fileMock.Setup(f => f.FileName).Returns("new-photo.jpg");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

        var model = new UpdateUserViewModel
        {
            Id = 1,
            Photo = fileMock.Object,
            StreetAddress = "Rue X",
            City = "Ville",
            PostalCode = "12345",
            AdditionalInfo = "",
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            PhoneNumber = "0123456789",
            ExistingPhoto = "old-photo.jpg"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(user);

        var tempDir = Path.Combine(Path.GetTempPath(), "tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        _envMock.Setup(e => e.WebRootPath).Returns(tempDir);

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        Assert.NotNull(user.Photo);
        Assert.NotEqual("old-photo.jpg", user.Photo);

        var photoPath = Path.Combine(tempDir, "images", "users", user.Photo);
        Assert.True(File.Exists(photoPath));

        File.Delete(photoPath);
        Directory.Delete(Path.GetDirectoryName(photoPath)!, true);
    }
    
    [Fact]
    public async Task UpdateUserAsync_UpdatesClaimsWithFirstNameLastNamePhoto()
    {
        // Arrange
        var user = CreateGauthierUser(1);
        user.Email = "gseyzeriat@example.com";
        user.Photo = "photo.jpg";

        var model = new UpdateUserViewModel
        {
            Id = 1,
            Photo = null,
            StreetAddress = "Rue X",
            City = "Ville",
            PostalCode = "12345",
            AdditionalInfo = "",
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            PhoneNumber = "0123456789",
            ExistingPhoto = "photo.jpg"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
        _userManagerMock.Setup(m => m.AddClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()))
            .Returns(Task.FromResult(IdentityResult.Success));
        _userManagerMock.Setup(m => m.RemoveClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()))
            .Returns(Task.FromResult(IdentityResult.Success));

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        _userManagerMock.Verify(m => m.GetClaimsAsync(user), Times.Once);
        _userManagerMock.Verify(m => m.RemoveClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()), Times.Once);
        _userManagerMock.Verify(m => m.AddClaimsAsync(user, It.Is<IEnumerable<Claim>>(claims =>
            claims.Any(c => c.Type == "FirstName" && c.Value == "Gauthier") &&
            claims.Any(c => c.Type == "LastName" && c.Value == "Seyzeriat") &&
            claims.Any(c => c.Type == "Email" && c.Value == "gseyzeriat@example.com") &&
            claims.Any(c => c.Type == "Photo" && c.Value == "photo.jpg")
        )), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUserAsync_WithAddressInfo_UpdatesUserAddress()
    {
        // Arrange
        var user = CreateGauthierUser(1);
        user.Address = new Address
        {
            Id = 1,
            StreetAddress = "Ancienne rue",
            AdditionalInfo = "Ancienne info",
            CityId = 10,
            City = new City
            {
                Id = 10,
                Name = "Old City",
                PostalCode = "99999",
                Addresses = new List<Address>(),
                Country = new Country { Id = 1, Name = "France", Cities = new List<City>() }
            }
        };

        var model = new UpdateUserViewModel
        {
            Id = 1,
            Photo = null,
            ExistingPhoto = "",
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            PhoneNumber = "0123456789",
            StreetAddress = "Nouvelle rue",
            City = "Paris",
            PostalCode = "75000",
            AdditionalInfo = "Apt 3B"
        };

        var france = new Country { Id = 1, Name = "France", Cities = new List<City>() };
        var newCity = new City
        {
            Id = 2,
            Name = "Paris",
            PostalCode = "75000",
            CountryId = 1,
            Country = france,
            Addresses = new List<Address>()
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(user);
        _countryRepoMock.Setup(r => r.GetByNameAsync("France")).ReturnsAsync(france);
        _cityRepoMock.Setup(r => r.GetByNameAndPostalCodeAsync("Paris", "75000")).ReturnsAsync(newCity);

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        Assert.NotNull(user.Address);
        Assert.Equal("Nouvelle rue", user.Address!.StreetAddress);
        Assert.Equal("Apt 3B", user.Address.AdditionalInfo);
        Assert.Equal(newCity.Id, user.Address.CityId);
        Assert.Equal("Paris", user.Address.City.Name);
        Assert.Equal("75000", user.Address.City.PostalCode);
        _addressRepoMock.Verify(r => r.UpdateAsync(user.Address), Times.Once);
        _addressRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUserAsync_CreatesCountryAndCity_WhenNotFound()
    {
        // Arrange
        var user = CreateGauthierUser(1);
        user.Address = new Address
        {
            StreetAddress = "1 rue test",
            City = new City
            {
                Name = "InconnueVille",
                PostalCode = "12345",
                Addresses = new List<Address>(),
                Country = new Country
                {
                    Name = "France",
                    Cities = new List<City>()
                }
            }
        };

        var model = new UpdateUserViewModel
        {
            Id = 1,
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            PhoneNumber = "0123456789",
            ExistingPhoto = "",
            Photo = null,
            StreetAddress = "1 rue test",
            City = "InconnueVille",
            PostalCode = "12345",
            AdditionalInfo = "Apt 99"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(user);
        _countryRepoMock.Setup(r => r.GetByNameAsync("France")).ReturnsAsync((Country)null!);
        _cityRepoMock.Setup(r => r.GetByNameAndPostalCodeAsync(model.City, model.PostalCode)).ReturnsAsync((City)null!);

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        _countryRepoMock.Verify(r => r.AddAsync(It.Is<Country>(c => c.Name == "France")), Times.Once);
        _countryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        _cityRepoMock.Verify(r => r.AddAsync(It.Is<City>(c => c.Name == "InconnueVille" && c.PostalCode == "12345")), Times.Once);
        _cityRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "Paris")]   
    [InlineData("Rue de Paris", "")]   
    [InlineData("", "")]              
    [InlineData(null, "Paris")]        
    [InlineData("Rue", null)]          
    public async Task UpdateUserAsync_EmptyStreetOrCity_DoesNotUpdateAddress(string? street, string? city)
    {
        // Arrange
        var user = CreateGauthierUser(1);
        user.Address = new Address
        {
            StreetAddress = "Ancienne rue",
            City = new City
            {
                Name = "Old City",
                PostalCode = "99999",
                Addresses = new List<Address>(),
                Country = new Country { Id = 1, Name = "France", Cities = new List<City>() }
            }
        };

        var model = new UpdateUserViewModel
        {
            Id = 1,
            FirstName = "Gauthier",
            LastName = "Seyzeriat",
            Bio = "Bio",
            PhoneNumber = "0123456789",
            ExistingPhoto = "",
            Photo = null,
            StreetAddress = street,
            City = city,
            PostalCode = "00000",
            AdditionalInfo = ""
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(user);

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        _countryRepoMock.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _cityRepoMock.Verify(r => r.GetByNameAndPostalCodeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _addressRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Address>()), Times.Never);
    }
}
