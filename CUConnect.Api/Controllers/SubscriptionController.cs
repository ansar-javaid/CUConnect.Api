using CUConnect.Database;
using CUConnect.Logic;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CUConnect.Api.Controllers
{
    [Route("api/subscription")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SubscriptionLogic _logic;

        public SubscriptionController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _logic = new SubscriptionLogic(_userManager);
        }

        /// <summary>
        /// Use to Follow(Like) / Unfollow/(Unlike) a specific profile page for a specific user.
        /// </summary>
        /// <param name="subscriptionView"></param>
        /// <returns></returns>

        [HttpPost, Route("FollowProfile")]
        public async Task<IActionResult> FollowProfile([FromBody] SubscriptionView subscriptionView)
        {
            var result = await _logic.FollowProfile(subscriptionView);
            return Ok(result);
        }


        /// <summary>
        /// Use to check the status of profile page, weather its is followed or not.
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        [HttpGet, Route("ProfileStatus")]
        public async Task<IActionResult> ProfileStatus([FromQuery][EmailAddress] string Email, [FromQuery] int ProfileId)
        {
            var result = await _logic.ProfileStatus(Email,ProfileId);
            return Ok(result);
        }
        

        /// <summary>
        /// Returns all the posts of a profile for a user, which user has subscribed/following
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpGet,Route("SubscribedProfilePosts")]
        public async Task<ActionResult<ProfileViewRES>> SubscribedProfilePosts([FromQuery][EmailAddress] string Email)
        {
            var result = await _logic.GetSubscribedProfilePosts(Email);
            return Ok(result);
        }
    }
}
