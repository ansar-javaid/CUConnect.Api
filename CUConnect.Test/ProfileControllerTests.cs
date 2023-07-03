using CUConnect.Api.Controllers;
using CUConnect.Database;
using CUConnect.Models.Repository;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CUConnect.Test
{
    public class ProfileControllerTests
    {
        private readonly ProfileController _controller;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IHostEnvironment> _environmentMock;
        private readonly Mock<IProfileREPO> _profileRepoMock;

        public ProfileControllerTests()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _environmentMock = new Mock<IHostEnvironment>();
            _profileRepoMock = new Mock<IProfileREPO>();
            _controller = new ProfileController(_userManagerMock.Object, _environmentMock.Object, _profileRepoMock.Object);
        }

        [Fact]
        public async Task GetAllDepartment_ReturnsOkResult()
        {
            // Arrange
            var departments = new List<DepartmentViewRES>
            {
                new DepartmentViewRES { DepartmentId = 1, Name = "Department 1" },
                new DepartmentViewRES { DepartmentId = 2, Name = "Department 2" }
            };
            _profileRepoMock.Setup(repo => repo.GetAllDepartment()).ReturnsAsync(departments);

            // Act
            var result = await _controller.GetDepartments();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<DepartmentViewRES>>(okResult.Value);
            Assert.Equal(departments, model);
        }

        [Fact]
        public async Task GetProfileById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var profileId = 0;
            var profile = new ProfileOnlyViewRES { ProfileID = profileId, ProfileTitle = "Profile 1", ProfileDescription = "This is Test Profiel", Path = "http://1.01/my.png" };
            _profileRepoMock.Setup(repo => repo.GetProfileOnly(profileId)).ReturnsAsync(profile);

            // Act
            var result = await _controller.GetProfileById(profileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsType<ProfileOnlyViewRES>(okResult.Value);
            Assert.Equal(profile, model);
        }

        [Fact]
        public async Task GetRegisteredUsers_ReturnsOkResult()
        {
            // Arrange
            var users = new List<RegisteredUsersViewRES>
            {
                new RegisteredUsersViewRES { Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
                new RegisteredUsersViewRES { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" }
            };
            _profileRepoMock.Setup(repo => repo.GetRegisteredUsers()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetRegisteredUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<RegisteredUsersViewRES>>(okResult.Value);
            Assert.Equal(users, model);
        }


        [Fact]
        public async Task GetAllProfiles_ReturnsAllProfiles()
        {
            // Arrange
            var profiles = new List<ProfileOnlyViewRES>
            {
                new ProfileOnlyViewRES { ProfileID = 1, ProfileTitle = "Profile 1", ProfileDescription = "Description 1", Path = "http://1.01/my.png"  },

                new ProfileOnlyViewRES { ProfileID = 2, ProfileTitle = "Profile 2", ProfileDescription = "Description 2", Path = "http://1.02/my.png" },

                new ProfileOnlyViewRES { ProfileID = 3, ProfileTitle = "Profile 3", ProfileDescription = "Description 3", Path = "http://1.03/my.png"  }
            };

            _profileRepoMock
                .Setup(repo => repo.GetAllProfiles())
                .ReturnsAsync(profiles);

            // Act
            var result = await _controller.GetAllProfiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<List<ProfileOnlyViewRES>>(okResult.Value);

            Assert.Equal(profiles.Count, model.Count);

            foreach (var profile in profiles)
            {
                var profileView = model.FirstOrDefault(p => p.ProfileID == profile.ProfileID);
                Assert.NotNull(profileView);
                Assert.Equal(profile.ProfileTitle, profileView.ProfileTitle);
                Assert.Equal(profile.ProfileDescription, profileView.ProfileDescription);
            }
        }

    }




}
