using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Index("PostsId", Name = "IX_Reactions_PostsID")]
    [Index("UserId", Name = "IX_Reactions_UserID")]
    public partial class Reaction
    {
        [Key]
        [Column("ReactionID")]
        public int ReactionId { get; set; }
        [Column("PostsID")]
        public int PostsId { get; set; }
        [Column("UserID")]
        public string UserId { get; set; } = null!;

        [ForeignKey("PostsId")]
        [InverseProperty("Reactions")]
        public virtual Post Posts { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("Reactions")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
