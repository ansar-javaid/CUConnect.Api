using CUConnect.Api.Controllers;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CUConnect.Test
{
    public class PostsControllerTests
    {
        private readonly PostsController _controller;
        private readonly Mock<IPostREPO> _postRepoMock;

        public PostsControllerTests()
        {
            _postRepoMock = new Mock<IPostREPO>();
            _controller = new PostsController(_postRepoMock.Object);
        }

        [Fact]
        public async Task GetPostsById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var profileId = 1;
            var posts = new List<PostViewRES>
            {
                new PostViewRES
                {
                    ProfileID = 1,
                    ProfileTitle = "Profile 1",
                    CoverPicture = new PostViewRES.Cover
                    {
                        ProfileImage = "http://example.com/profile1.jpg"
                    },
                    PostID = 1,
                    PostDescription = "Post 1",
                    PostsCreatedOn = DateTime.Now.AddDays(1),
                    FilePath = new List<PostViewRES.Files>
        {
            new PostViewRES.Files
            {
                Path = "http://example.com/post1_file1.jpg"
            },
            new PostViewRES.Files
            {
                Path = "http://example.com/post1_file2.jpg"
            }
        }
    },
    new PostViewRES
    {
        ProfileID = 2,
        ProfileTitle = "Profile 2",
        CoverPicture = new PostViewRES.Cover
        {
            ProfileImage = "http://example.com/profile2.jpg"
        },
        PostID = 2,
        PostDescription = "Post 2",
        PostsCreatedOn = DateTime.Now.AddDays(12),
        FilePath = new List<PostViewRES.Files>
        {
            new PostViewRES.Files
            {
                Path = "http://example.com/post2_file1.jpg"
            }
        }
    }
};

            _postRepoMock.Setup(repo => repo.GetPosts(profileId)).ReturnsAsync(posts);

            // Act
            var result = await _controller.GetPostsById(profileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPostsById_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var invalidProfileId = -1;
            _postRepoMock.Setup(repo => repo.GetPosts(invalidProfileId)).ReturnsAsync(new List<PostViewRES>());

            // Act
            var result = await _controller.GetPostsById(invalidProfileId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePost_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var postsView = new PostsView
            {
                ProfileID = 1,
                Description = "New post Desciption",
            };

            var expectedResult = new
            {
                Status = true,
                Msg = "No Files found to Upload",
                Uploaded = false
            };

            _postRepoMock.Setup(repo => repo.CreatPost(postsView))
                .ReturnsAsync(new ObjectResult(expectedResult)
                {
                    StatusCode = StatusCodes.Status201Created
                });

            // Act
            var result = await _controller.CreatePost(postsView);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ObjectResult>(okResult.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
        }

        [Fact]
        public async Task CreatePost_WithInvalidData_ReturnsBadRequestResult()
        {
            // Arrange
            var postsView = new PostsView
            {
                ProfileID = -1,
                Description = null,
                Files = null
            };
            var expectedResult = new
            {
                Status = false,
                Msg = "Invalid Data",
                Uploaded = false
            };

            _postRepoMock.Setup(repo => repo.CreatPost(postsView))
               .ReturnsAsync(new ObjectResult(expectedResult)
               {
                   StatusCode = StatusCodes.Status400BadRequest
               });

            // Act
            var result = await _controller.CreatePost(postsView);

            // Assert
            Assert.NotNull(result);
            var status = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ObjectResult>(status.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }



    }
}
