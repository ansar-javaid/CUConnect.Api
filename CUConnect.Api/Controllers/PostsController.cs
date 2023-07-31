using CUConnect.Models;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        //Dependencies-------------------------------------
        private readonly IPostREPO _post;

        //Constructor---------------------------------------
        public PostsController(IPostREPO post)
        {
            _post = post;
        }

        #region


        /// <summary>
        /// Get all the related Posts of a profile by accpeting the Profile ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPostsByProfile")]
        public async Task<ActionResult<List<PostViewRES>>> GetPostsById(int id)
        {
            var result = await _post.GetPosts(id);
            if (result.Count == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }



        /// <summary>
        /// Get a single Post by Post Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPostbyId")]
        public async Task<ActionResult<List<PostViewRES>>> GetPostById(int id)
        {
            var result = await _post.GetPost(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }




        /// <summary>
        /// Creat a Post for related Profile with the support of multiple file upload
        /// </summary>
        /// <param name="postsView"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost, Route("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] PostsView postsView)
        {
            var result = await _post.CreatPost(postsView);
            return Ok(result);
        }

        #endregion
    }
}
