using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUConnect.Database.Entities
{
    public partial class Subscription
    {
        [Key]
        [Column("SubscritionID")]
        public int SubscritionId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; } = null!;

        [ForeignKey("ProfileId")]
        [InverseProperty("Subscriptions")]
        public virtual Profile Profile { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("Subscriptions")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
