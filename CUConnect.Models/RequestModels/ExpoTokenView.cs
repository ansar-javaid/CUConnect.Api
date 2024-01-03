using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class ExpoTokenView

    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
