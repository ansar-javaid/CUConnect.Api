using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CUConnect.Models.ResponseModels.PostViewRES;

namespace CUConnect.Logic
{
    public class SubscriptionLogic : ControllerBase, ISubscriptionREPO
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

        #region React on Post
        public async Task<IActionResult> ReactOnPost(ReactionView reactionView)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var user = await _userManager.FindByEmailAsync(reactionView.Email);
                var postResult = _dbContext.Posts.Where(x => x.PostId.Equals(reactionView.PostID)).FirstOrDefault();
                if (user != null && postResult != null)
                {
                    var reactionResult = _dbContext.Reactions
                        .Where(x => x.PostsId.Equals(reactionView.PostID)
                        && x.UserId.Equals(user.Id)).FirstOrDefault();
                    if (reactionResult != null)
                    {
                        _dbContext.Reactions.Remove(reactionResult);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { Status = false, Msg = "Un-Reacted", Post = postResult.PostId });

                    }
                    else
                    {
                        var reaction = new Reaction()
                        {
                            PostsId = reactionView.PostID,
                            UserId = user.Id,
                        };
                        _dbContext.Reactions.Add(reaction);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { status = true, Msg = "Reacted", Post = postResult.PostId });
                    }
                }
                return StatusCode(StatusCodes.Status404NotFound, new { status = false, Msg = "User or Post not found" });
            }
        }
        #endregion

        //=====================================================================================================================================================

        #region Profile Status
        public async Task<IActionResult> ProfileStatus(SubscriptionView subscriptionView)
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
                        return StatusCode(StatusCodes.Status200OK, new { Status = true, Msg = "Followed", Profile = profieResult.Title });
                    }
                    return StatusCode(StatusCodes.Status404NotFound, new { Status = false, Msg = "Unfollowed", Profile = profieResult.Title });
                }
                return StatusCode(StatusCodes.Status404NotFound, new { status = false, Msg = "User or Profile not found" });
            }
        }
        #endregion
        //=====================================================================================================================================================
        #region Post Reaction Status
        public async Task<IActionResult> ReactionStatus(ReactionView reactionView)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var user = await _userManager.FindByEmailAsync(reactionView.Email);
                var postResult = _dbContext.Posts.Where(x => x.PostId.Equals(reactionView.PostID)).FirstOrDefault();
                if (user != null && postResult != null)
                {
                    var reactionResult = _dbContext.Reactions
                       .Where(x => x.PostsId.Equals(reactionView.PostID)
                       && x.UserId.Equals(user.Id)).FirstOrDefault();
                    if (reactionResult != null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { status = true, Msg = "Reacted", Post = postResult.PostId });
                    }
                    return StatusCode(StatusCodes.Status404NotFound, new { Status = false, Msg = "Un-Reacted", Post = postResult.PostId });
                }
                return StatusCode(StatusCodes.Status404NotFound, new { status = false, Msg = "User or Profile not found" });
            }
        }
        #endregion
        //=====================================================================================================================================================

        public async Task<List<PostViewRES>> GetSubscribedProfilePosts(string Email)
        {
            //Only use(host) when the files are on same server wwwroot
            var user = await _userManager.FindByEmailAsync(Email);
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";

            using (var _dbContext = new CUConnectDBContext())
            {

                var profile = await _dbContext.Profiles
                    .Include(x => x.Subscriptions)
                    .Include(y => y.Posts).ThenInclude(z => z.Documents)
                    .Include(x => x.Posts).ThenInclude(r => r.Reactions)
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
                                         .Select(doc => new Cover() { ProfileImage = doc.Path })
                                         .FirstOrDefault(),

                         PostID = z.PostId,
                         PostDescription = z.Description,
                         PostsCreatedOn = z.PostedOn,
                         Reaction = z.Reactions.Any(r => r.UserId == user.Id),
                         TotalReactions = z.Reactions.Select(r => r.PostsId.Equals(r.PostsId)).Count(),
                         FilePath = z.Documents.Select(x => new PostViewRES.Files()
                         {
                             Path = x.Path
                         }).ToList()

                     }).OrderByDescending(p => p.TotalReactions)
                       .ToListAsync();
                return profile;
            }
        }
        //=====================================================================================================================================================
    }
}

