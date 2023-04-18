using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class ProfileView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int? DepartmentId { get; set; } = null;

        public int? ClassId { get; set; } = null;

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }




    }
}
