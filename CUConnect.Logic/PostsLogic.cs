using AutoMapper;
using CUConnect.Database.Entities;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Logic
{
    public class PostsLogic:ControllerBase
    {
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            using(var _dbContext=new CUConnectDBContext())
            {
                return (await _dbContext.Posts.ToListAsync());
            }
        }

        public async Task<IActionResult> CreatPost(PostsView postsView)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var post = new Post()
                {
                    ProfileId = postsView.ProfileID,
                    Description = postsView.Description,
                    PostedOn = DateTime.Now,
                };
                _dbContext.Posts.Add(post);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, new { Status = "Posted" });
            }
        }

    }
}
