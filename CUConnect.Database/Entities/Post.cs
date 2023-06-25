using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    public partial class Post
    {
        public Post()
        {
            Documents = new HashSet<Document>();
            PostTags = new HashSet<PostTag>();
            Reactions = new HashSet<Reaction>();
        }

        [Key]
        [Column("PostID")]
        public int PostId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        public string Description { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime PostedOn { get; set; }

        [ForeignKey("ProfileId")]
        [InverseProperty("Posts")]
        public virtual Profile Profile { get; set; } = null!;
        [InverseProperty("Posts")]
        public virtual ICollection<Document> Documents { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<PostTag> PostTags { get; set; }
        [InverseProperty("Posts")]
        public virtual ICollection<Reaction> Reactions { get; set; }
    }
}
