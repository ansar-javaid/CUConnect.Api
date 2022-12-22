using CUConnect.Logic;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostsLogic _postsLogic;
        public PostsController()
        {
            _postsLogic = new PostsLogic();
        }

        [HttpGet,Route("GetAllPosts")]
        public IActionResult GetAllPosts()
        {
            return Ok(new { Data = _postsLogic.GetAllPosts() });
        }

        [HttpPost,Route("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] PostsView postsView)
        {
            var result =await _postsLogic.CreatPost(postsView);
            return Ok(result);
        }
    }
}
