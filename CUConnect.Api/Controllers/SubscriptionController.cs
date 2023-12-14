using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CUConnect.Api.Controllers
{
    [Route("api/subscription")]
    [ApiController]
    //[Authorize(Roles = nameof(Roles.User))]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionREPO _subscription;

        public SubscriptionController(ISubscriptionREPO subscription)
        {
            _subscription = subscription;
        }

        /// <summary>
        /// Use to Follow(Like) / Unfollow/(Unlike) a specific profile page for a specific user.
        /// </summary>
        /// <param name="subscriptionView"></param>
        /// <returns></returns>

        [HttpPost, Route("FollowProfile")]
        public async Task<IActionResult> FollowProfile([FromBody] SubscriptionView subscriptionView)
        {
            var result = await _subscription.FollowProfile(subscriptionView);
            return Ok(result);
        }


        /// <summary>
        /// Use to React/ Un-React(Unlike) a specific post for a specific user.
        /// </summary>
        /// <param name="reactionView"></param>
        /// <returns></returns>
        [HttpPost, Route("ReactOnPost")]
        public async Task<IActionResult> ReactOnPost([FromBody] ReactionView reactionView)
        {
            var result = await _subscription.ReactOnPost(reactionView);
            return Ok(result);
        }



        /// <summary>
        /// Use to check the status of profile page, weather its is followed or not.
        /// </summary>
        /// <param name="subscriptionView"></param>
        /// <returns></returns>
        [HttpGet, Route("ProfileStatus")]
        public async Task<IActionResult> ProfileStatus([FromQuery] SubscriptionView subscriptionView)
        {
            var result = await _subscription.ProfileStatus(subscriptionView);
            return Ok(result);
        }




        /// <summary>
        ///  Use to check the status of reaction on post, weather its is Reacted or not.
        /// </summary>
        /// <param name="reactionView"></param>
        /// <returns></returns>
        [HttpGet, Route("ReactionStatus")]
        public async Task<IActionResult> ReactionStatus([FromQuery] ReactionView reactionView)
        {
            var result = await _subscription.ReactionStatus(reactionView);
            return Ok(result);
        }



        /// <summary>
        /// Returns all the posts of all profiles for a user, with reactions and their status. Pass the page number for Data pagination
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet, Route("NewsFeedPosts")]
        public async Task<ActionResult<ProfileViewRES>> NewsFeedPosts([FromQuery][EmailAddress] string Email, [FromQuery] int page)
        {
            var result = await _subscription.GetNewsFeedPosts(Email, page);
            return Ok(result);
        }



        /// <summary>
        /// Returns Only the Posts of A Specific Specific Profile, with Reactions on the Post, returns all the posts with the Post-Id and reactions,
        /// with Email. Pass the page number for Data pagination.
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="Email"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet, Route("SubscribedProfilePosts")]
        public async Task<ActionResult<ProfileViewRES>> SubscribedProfilePosts([FromQuery] int profileId, [FromQuery][EmailAddress] string Email, [FromQuery] int page)
        {
            var result = await _subscription.GetSubscribedProfilePosts(profileId, Email, page);
            return Ok(result);
        }
    }

}
