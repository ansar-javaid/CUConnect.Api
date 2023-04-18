using CUConnect.Logic;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        //Dependencies-------------------------------------
        private readonly PostsLogic _postsLogic;
        private readonly IHostEnvironment _environment;

        //Constructor---------------------------------------
        public PostsController(IHostEnvironment environment)
        {
            _environment = environment;
            _postsLogic = new PostsLogic(_environment);
        }

        #region

        /// <summary>
        /// Get All Posts: everything
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllPosts")]
        public IActionResult GetAllPosts()
        {
            return Ok(new { Data = _postsLogic.GetAllPosts() });
        }

        /// <summary>
        /// Creat a Post for related Profile with the support of multiple file upload
        /// </summary>
        /// <param name="postsView"></param>
        /// <returns></returns>

        [HttpPost, Route("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] PostsView postsView)
        {
            var result = await _postsLogic.CreatPost(postsView);
            return Ok(result);
        }



        /// <summary>
        /// Get all the related Posts of a profile by accpeting the Profile ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPostsByProfile")]
        public async Task<ActionResult<PostViewRES>> GetPostsById(int id)
        {
            var result = await _postsLogic.GetPosts(id);
            return Ok(new { Database = result != null ? result : null });
        }

        #endregion
    }
}
