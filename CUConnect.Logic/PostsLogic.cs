using AutoMapper;
using CUConnect.Database.Entities;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Logic
{
    public class PostsLogic:ControllerBase
    {
        private readonly FileUploadLogic _fileUploadLogic;
        public PostsLogic(IHostEnvironment environment)
        {
            _fileUploadLogic = new FileUploadLogic(environment);
        }
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            using(var _dbContext=new CUConnectDBContext())
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
                    return StatusCode(StatusCodes.Status201Created, new { Status = true, Msg = "No Files found to Upload", Uploaded = false });

                }
            }
            if (postsView.Files.Count > 5)
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = false, Msg = "Files Limit Excedded: 5 allowed", FileUploaded=false });
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
                return StatusCode(StatusCodes.Status201Created, new { Status = result.status, FileUploaded = result.status, TotalFiles=result.totalfiles, Size=result.size });
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
                var profile = await _dbContext.Profiles.Include(y=>y.Posts).ThenInclude(z=>z.Documents).Where(x => x.ProfileId == profileId)
                     .SelectMany( z=>z.Posts.DefaultIfEmpty() ,(y,z) => new PostViewRES()
                     {
                         ProfileID=y.ProfileId,
                         ProfileTitle=y.Title,
                                        
                         PostID= z.PostId,
                         PostDescription =z.Description,
                         PostsCreatedOn=z.PostedOn,
                         FilePath=z.Documents.Select(x=> new PostViewRES.Files()
                         {
                             Path=host+x.Name
                         }).ToList()
                         
                     }).ToListAsync();
                return profile;
            }
        }

    }
}
