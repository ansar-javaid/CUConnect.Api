using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class PostsView
    {
        [Required]
        public int ProfileID { get; set; }

        [Required]
        public string Description { get; set; }

        [DataType(DataType.Upload)]
        public List<IFormFile>? Files { get; set; }
    }
}
