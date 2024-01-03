using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Index("ProfileId", Name = "IX_Subscriptions_ProfileID")]
    [Index("UserId", Name = "IX_Subscriptions_UserID")]
    public partial class Subscription
    {
        [Key]
        [Column("SubscritionID")]
        public int SubscritionId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("UserID")]
        public string UserId { get; set; } = null!;

        [ForeignKey("ProfileId")]
        [InverseProperty("Subscriptions")]
        public virtual Profile Profile { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("Subscriptions")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
