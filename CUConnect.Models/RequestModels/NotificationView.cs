using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class NotificationView
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
