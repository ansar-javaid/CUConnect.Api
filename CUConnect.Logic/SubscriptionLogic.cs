using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CUConnect.Models.ResponseModels.PostViewRES;

namespace CUConnect.Logic
{
    public class SubscriptionLogic : ControllerBase
    {
        //Identity
        private readonly UserManager<AppUser> _userManager;
        //Constructor--------------------------------------------------
        public SubscriptionLogic(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //Methods===============================================================================================================================================

        #region Follow/Unfollow Profile
        public async Task<IActionResult> FollowProfile(SubscriptionView subscriptionView)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var user = await _userManager.FindByEmailAsync(subscriptionView.Email);
                var profieResult = _dbContext.Profiles.Where(x => x.ProfileId.Equals(subscriptionView.ProfileID)).FirstOrDefault();

                if (user != null && profieResult != null)
                {
                    var subscriptionResult = _dbContext.Subscriptions
                        .Where(x => x.ProfileId.Equals(subscriptionView.ProfileID)
                        && x.UserId.Equals(user.Id)).FirstOrDefault();
                    if (subscriptionResult != null)
                    {
                        _dbContext.Subscriptions.Remove(subscriptionResult);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { Status = false, Msg = "Unfollowing", Profile = profieResult.Title });
                    }
                    else
                    {
                        var subscription = new Subscription()
                        {
                            ProfileId = subscriptionView.ProfileID,
                            UserId = user.Id,
                        };
                        _dbContext.Subscriptions.Add(subscription);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { status = true, Msg = "Following", Profile = profieResult.Title });
                    }

                }
                return StatusCode(StatusCodes.Status404NotFound, new { status = false, Msg = "User or Profile not found" });
            }
        }
        #endregion

        //=====================================================================================================================================================
        public async Task<IActionResult> ProfileStatus(string Email, int ProfileId)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var user = await _userManager.FindByEmailAsync(Email);
                var profieResult = _dbContext.Profiles.Where(x => x.ProfileId.Equals(ProfileId)).FirstOrDefault();

                if (user != null && profieResult != null)
                {
                    var subscriptionResult = _dbContext.Subscriptions
                        .Where(x => x.ProfileId.Equals(ProfileId)
                        && x.UserId.Equals(user.Id)).FirstOrDefault();
                    if (subscriptionResult != null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { Status = true, Msg = "Followed", Profile = profieResult.Title });
                    }
                    return StatusCode(StatusCodes.Status404NotFound, new { Status = false, Msg = "Unfollowed", Profile = profieResult.Title });
                }
                return StatusCode(StatusCodes.Status404NotFound, new { status = false, Msg = "User or Profile not found" });
            }
        }

        //=====================================================================================================================================================

        public async Task<List<PostViewRES>> GetSubscribedProfilePosts(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";
            using (var _dbContext = new CUConnectDBContext())
            {

                var profile = await _dbContext.Profiles
                    .Include(x => x.Subscriptions)
                    .Include(y => y.Posts).ThenInclude(z => z.Documents)
                    .Select(x => new
                    {
                        profiles = x,
                        data = x.Subscriptions
                                .Where(n => n.UserId.Equals(user.Id))//filter subscription node for:id
                                .Select(y => y.ProfileId)
                    })
                     .SelectMany(z => z.profiles.Posts.Where(i => z.data.Any(c => c == i.ProfileId)), (y, z) => new PostViewRES()
                     {
                         ProfileID = y.profiles.ProfileId,
                         ProfileTitle = y.profiles.Title,
                         CoverPicture = y.profiles.Documents
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

                     }).OrderByDescending(p => p.PostsCreatedOn) // Order by PostsCreatedOn property in descending order
                       .ToListAsync();
                return profile;
            }
        }
    }
}

