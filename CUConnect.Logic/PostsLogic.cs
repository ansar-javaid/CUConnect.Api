using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Logic.Notifications;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static CUConnect.Models.ResponseModels.PostViewRES;

namespace CUConnect.Logic
{
    public class PostsLogic : ControllerBase, IPostREPO
    {
        private readonly FileUploadLogic _fileUploadLogic;
        private IHubContext<NotificationHub> _hubContext;
        //Identity
        private readonly UserManager<AppUser> _userManager;
        //Constructor--------------------------------------------------
        public PostsLogic(IHostEnvironment environment, IHubContext<NotificationHub> hubContext, UserManager<AppUser> userManager)
        {
            _fileUploadLogic = new FileUploadLogic(environment);
            _hubContext = hubContext;
            _userManager = userManager;
        }


        public async Task<IActionResult> CreatPost(PostsView postsView)
        {
            if (postsView.ProfileID < 0 || postsView.Description == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = false, Msg = "Invalid Data", FileUploaded = false });
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



        public async Task<List<PostViewRES>> GetPosts(int profileId)
        {
            //var user = await _userManager.FindByEmailAsync(Email);
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";
            using (var _dbContext = new CUConnectDBContext())
            {
                //var profile = await _dbContext.Profiles.Where(x => x.ProfileId == profileId).Include(x => x.Posts).ToListAsync();
                return await _dbContext.Profiles
                    .Include(y => y.Posts)
                    .ThenInclude(z => z.Documents)
                    .Where(x => x.ProfileId.Equals(profileId))
                     .SelectMany(z => z.Posts.DefaultIfEmpty(), (y, z) => new PostViewRES()
                     {
                         ProfileID = y.ProfileId,
                         ProfileTitle = y.Title,
                         CoverPicture = y.Documents
                                         .Where(doc => doc.ProfileId.Equals(z.ProfileId))
                                         .Select(doc => new Cover() { ProfileImage = host + doc.Name })
                                         .FirstOrDefault(),

                         PostID = z.PostId,
                         PostDescription = z.Description,
                         PostsCreatedOn = z.PostedOn,
                         FilePath = z.Documents.Select(x => new PostViewRES.Files()
                         {
                             Path = host + x.Name
                         }).ToList()

                     }).ToListAsync();
            }
        }


        public async Task<ActionResult<PostViewRES>> GetPost(int postId)
        {
            //var user = await _userManager.FindByEmailAsync(Email);
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";
            using (var _dbContext = new CUConnectDBContext())
            {
                //var profile = await _dbContext.Profiles.Where(x => x.ProfileId == profileId).Include(x => x.Posts).ToListAsync();
                return await _dbContext.Posts
                    .Include(z => z.Documents)
                    .Where(x => x.PostId.Equals(postId))
                     .Select(z => new PostViewRES()
                     {

                         PostID = z.PostId,
                         PostDescription = z.Description,
                         PostsCreatedOn = z.PostedOn,
                         FilePath = z.Documents.Select(x => new PostViewRES.Files()
                         {
                             Path = host + x.Name
                         }).ToList()

                     }).FirstOrDefaultAsync();
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
