using CUConnect.Database;
using CUConnect.Logic;
using CUConnect.Models;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace CUConnect.Test
{
    public class AuthenticationLogicTests
    {
        private readonly AuthenticationLogic _logic;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

        public AuthenticationLogicTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _logic = new AuthenticationLogic(_configurationMock.Object, _userManagerMock.Object, _roleManagerMock.Object);
        }

        [Fact]
        public async Task Register_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new RegisterUserView
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = 1,
                Password = "Test123@"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), request.Password)).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(Roles.User.ToString())).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), Roles.User.ToString())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddClaimAsync(It.IsAny<AppUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _logic.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.Equal("Success", response["Status"]);
            Assert.Equal("User created successfully!", response["Message"]);


        }

        [Fact]
        public async Task Register_WithExistingUser_ReturnsInternalServerError()
        {
            // Arrange
            var request = new RegisterUserView
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = 1,
                Password = "Test123@"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(new AppUser());

            // Act
            var result = await _logic.Register(request);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("User already exists!", internalServerErrorResult.Value);
        }

        [Fact]
        public async Task Register_WithFailedUserCreation_ReturnsInternalServerError()
        {
            // Arrange
            var request = new RegisterUserView
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = 1,
                Password = "Test123@"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), request.Password)).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _logic.Register(request);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("User creation failed! Please check user details and try again.", internalServerErrorResult.Value);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var request = new LoginUserView
            {
                Email = "test@example.com",
                Password = "Test123@"
            };

            var user = new AppUser { Email = request.Email };
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, Roles.User.ToString()) };

            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.GetClaimsAsync(user)).ReturnsAsync(claims);

            // Act
            var result = await _logic.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var request = new LoginUserView
            {
                Email = "test@example.com",
                Password = "Test123@"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((AppUser)null);

            // Act
            var result = await _logic.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
            Assert.Equal("Check User Name or Password again!", unauthorizedResult.Value);
        }

        // Additional tests for other methods can be added here
    }
}
