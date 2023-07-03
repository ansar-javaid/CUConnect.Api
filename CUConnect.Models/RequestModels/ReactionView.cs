using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class ReactionView
    {
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is not valid")]
        public string Email { get; set; }

        [Required]
        public int PostID { get; set; }
    }
}
