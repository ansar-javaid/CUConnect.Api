using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUConnect.Database.Entities
{
    public partial class AspNetUserToken
    {
        [Key]
        public string UserId { get; set; } = null!;
        [Key]
        public string LoginProvider { get; set; } = null!;
        [Key]
        public string Name { get; set; } = null!;
        public string? Value { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("AspNetUserTokens")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
