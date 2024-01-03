using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Index("PostId", Name = "IX_PostTags_PostID")]
    [Index("TagId", Name = "IX_PostTags_TagID")]
    public partial class PostTag
    {
        [Key]
        [Column("PostTagID")]
        public int PostTagId { get; set; }
        [Column("TagID")]
        public int TagId { get; set; }
        [Column("PostID")]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        [InverseProperty("PostTags")]
        public virtual Post Post { get; set; } = null!;
        [ForeignKey("TagId")]
        [InverseProperty("PostTags")]
        public virtual Tag Tag { get; set; } = null!;
    }
}
