﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUConnect.Database.Entities
{
    public partial class Tag
    {
        public Tag()
        {
            PostTags = new HashSet<PostTag>();
        }

        [Key]
        [Column("TagID")]
        public int TagId { get; set; }
        public string Name { get; set; } = null!;

        [InverseProperty("Tag")]
        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}
