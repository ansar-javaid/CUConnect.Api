using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Models.RequestModels
{
    public class PostsView
    {
        [Required]
        public int ProfileID { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
