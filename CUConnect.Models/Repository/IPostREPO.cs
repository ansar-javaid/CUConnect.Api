﻿using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Models.Repository
{
    public interface IPostREPO
    {
        #region GET Methods
        public Task<List<PostViewRES>> GetPosts(int profileId);
        #endregion

        #region POST Methods
        public Task<IActionResult> CreatPost(PostsView postsView);
        #endregion

    }
}
