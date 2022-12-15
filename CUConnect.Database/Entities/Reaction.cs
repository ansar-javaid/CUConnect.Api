using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    public partial class Reaction
    {
        [Key]
        [Column("ReactionID")]
        public int ReactionId { get; set; }
        [Column("PostsID")]
        public int PostsId { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; } = null!;
        public int ReactinType { get; set; }

        [ForeignKey("PostsId")]
        [InverseProperty("Reactions")]
        public virtual Post Posts { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("Reactions")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
