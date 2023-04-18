using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class SubscriptionView
    {
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is not valid")]
        public string Email { get; set; }

        [Required]
        public int ProfileID { get; set; }
    }
}
