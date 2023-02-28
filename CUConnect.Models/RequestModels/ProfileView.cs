using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Models.RequestModels
{
    public class ProfileView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } 

        public int? DepartmentId { get; set; }=null;

        public int? ClassId { get; set; } = null;

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }




    }
}
