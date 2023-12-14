using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Models.Repository
{
    public interface ISubscriptionREPO
    {
        public Task<IActionResult> FollowProfile(SubscriptionView subscriptionView);
        public Task<IActionResult> ReactOnPost(ReactionView reactionView);
        public Task<IActionResult> ProfileStatus(SubscriptionView subscriptionView);
        public Task<IActionResult> ReactionStatus(ReactionView reactionView);
        public Task<List<PostViewRES>> GetNewsFeedPosts(string Email, int page);
        public Task<List<PostViewRES>> GetSubscribedProfilePosts(int profileId, string Email, int page);
    }
}
