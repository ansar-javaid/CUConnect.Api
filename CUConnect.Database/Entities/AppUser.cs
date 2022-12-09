using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Database
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        public string FirstName { get; set; }

        
        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        public string LastName { get; set; }

        [Required]
        [Range(0,1)] // 0 is female, 1 is male
        public int Gender { get; set; }

        [Required]
        public DateTime UserJoined { get; set; }

        public AppUser()
        {
            this.UserJoined = DateTime.Now;
        }

    }
}
