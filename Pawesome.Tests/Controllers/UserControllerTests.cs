using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pawesome.Controllers;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.User;
using System.Security.Claims;
using Pawesome.Models.ViewModels.Pet;

namespace Pawesome.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;

    private readonly UserController _controller;

    public UserControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

        _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null!, null!, null!, null!);

        _controller = new UserController(
            _userServiceMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object
        );
    }

    private void SetUserContext(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "mock");
        var userPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userPrincipal }
        };
    }

    [Fact]
    public async Task Index_UserNotLoggedIn_RedirectsToLogin()
    {
        // Arrange
        _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null!);

        // Act
        var result = await _controller.Index();

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Auth", redirect.ControllerName);
    }

    [Fact]
    public async Task Index_UserFound_ReturnsView()
    {
        // Arrange
        SetUserContext("1");
        _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

        _userServiceMock.Setup(s => s.GetUserProfileAsync(1))
            .ReturnsAsync(new ProfileViewModel
            {
                Id = 1,
                FirstName = "Gauthier",
                LastName = "Seyzeriat",
                Email = "g@example.com",
                PhoneNumber = "0123456789",
                Bio = "Bio",
                Photo = "photo.jpg",
                Status = "Actif",
                IsVerified = true,
                BalanceAccount = 0,
                Rating = 4.5f,
                CreatedAt = DateTime.UtcNow,
                Street = "Rue Test",
                City = "Lyon",
                PostalCode = "69000",
                Country = "France",
                Pets = new()
            });

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ProfileViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task Edit_GetUserNotFound_ReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(s => s.GetUserForEditAsync(42)).ReturnsAsync((UpdateUserViewModel)null!);

        // Act
        var result = await _controller.Edit(42);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Profile_OtherUserFound_ReturnsView()
    {
        // Arrange
        SetUserContext("1");
        _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

        _userServiceMock.Setup(s => s.GetPublicUserProfileAsync(2, 1)).ReturnsAsync(new PublicProfileViewModel
        {
            Id = 2,
            FullName = "Autre utilisateur",
            CreatedAt = DateTime.UtcNow,
            IsCurrentUser = false,
            Pets = new List<PetViewModel>()
        });

        // Act
        var result = await _controller.Profile(2);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<PublicProfileViewModel>(viewResult.Model);
    }
}
