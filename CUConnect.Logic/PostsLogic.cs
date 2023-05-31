﻿using CUConnect.Database.Entities;
using CUConnect.Logic.Notifications;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CUConnect.Logic
{
    public class PostsLogic : ControllerBase
    {
        private readonly FileUploadLogic _fileUploadLogic;
        private IHubContext<NotificationHub> _hubContext;
        public PostsLogic(IHostEnvironment environment, IHubContext<NotificationHub> hubContext)
        {
            _fileUploadLogic = new FileUploadLogic(environment);
            _hubContext = hubContext;
        }
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                return (await _dbContext.Posts.ToListAsync());
            }
        }

        public async Task<IActionResult> CreatPost(PostsView postsView)
        {
            if (postsView.Files == null)
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
                    await Send(new NotificationRES() { ProfileName = "Important! Notification", PostID = 0, Message = postsView.Description });
                    return StatusCode(StatusCodes.Status201Created, new { Status = true, Msg = "No Files found to Upload", Uploaded = false });

                }
            }
            if (postsView.Files.Count > 5)
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = false, Msg = "Files Limit Excedded: 5 allowed", FileUploaded = false });
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
                var result = await _fileUploadLogic.Upload(postsView.Files, post);
                await Send(new NotificationRES() { ProfileName = "Important! Notification", PostID = 0, Message = postsView.Description });
                return StatusCode(StatusCodes.Status201Created, new { Status = result.status, FileUploaded = result.status, TotalFiles = result.totalFiles, Size = result.size });
            }
        }

        /*

                public async Task<List<PostViewRES>> GetPosts(int profileId)
                {
                    IHttpContextAccessor httpContext = new HttpContextAccessor();
                    var request = httpContext.HttpContext.Request;
                    var host = $"{request.Scheme}://{request.Host}";
                    using (var _dbContext = new CUConnectDBContext())
                    {
                        //var profile = await _dbContext.Profiles.Where(x => x.ProfileId == profileId).Include(x => x.Posts).ToListAsync();
                        return await (from profile in _dbContext.Profiles
                                      where (profile.ProfileId == profileId)
                                      join post in _dbContext.Posts
                                      on profile.ProfileId equals post.ProfileId into pe
                                      from x in pe.DefaultIfEmpty()
                                      join z in _dbContext.Documents
                                      on x.PostId equals z.PostsId into pec
                                      from y in pec.DefaultIfEmpty()
                                      select new PostViewRES
                                      {
                                          ProfileID = profile.ProfileId,
                                          ProfileTitle = profile.Title,


                                          PostID = x.PostId,
                                          PostDescription = x.Description,
                                          PostsCreatedOn = x.PostedOn,
                                          FilePath = x.Documents.Select(c => new PostViewRES.Files() { Path = host + c.Path }).ToList()
                                      }).ToListAsync();
                    }
                }

        */

        public async Task<List<PostViewRES>> GetPosts(int profileId)
        {
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";
            using (var _dbContext = new CUConnectDBContext())
            {
                //var profile = await _dbContext.Profiles.Where(x => x.ProfileId == profileId).Include(x => x.Posts).ToListAsync();
                var profile = await _dbContext.Profiles.Include(y => y.Posts).ThenInclude(z => z.Documents).Where(x => x.ProfileId == profileId)
                     .SelectMany(z => z.Posts.DefaultIfEmpty(), (y, z) => new PostViewRES()
                     {
                         ProfileID = y.ProfileId,
                         ProfileTitle = y.Title,

                         PostID = z.PostId,
                         PostDescription = z.Description,
                         PostsCreatedOn = z.PostedOn,
                         FilePath = z.Documents.Select(x => new PostViewRES.Files()
                         {
                             Path = host + x.Name
                         }).ToList()

                     }).ToListAsync();
                return profile;
            }
        }

        private async Task<IActionResult> Send(NotificationRES notification)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("Notification", notification);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

    }
}
