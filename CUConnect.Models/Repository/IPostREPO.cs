using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Models.Repository
{
    public interface IPostREPO
    {
        #region GET Methods
        public Task<List<PostViewRES>> GetPosts(int profileId);
        public Task<ActionResult<PostViewRES>> GetPost(int postId);
        public Task<ActionResult<ImagesViewRES>> LoadAllImages();
        #endregion

        #region POST Methods
        public Task<IActionResult> CreatPost(PostsView postsView);
        #endregion

        #region DELETE Methods
        public Task<IActionResult> DeletePost(int postId);
        #endregion

    }
}
