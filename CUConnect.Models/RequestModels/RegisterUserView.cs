﻿using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class RegisterUserView
    {

        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        public string FirstName { get; set; }


        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        public string LastName { get; set; }

        [Required]
        [Range(0, 1)] // 0 is female, 1 is male
        public int Gender { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is not valid")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
