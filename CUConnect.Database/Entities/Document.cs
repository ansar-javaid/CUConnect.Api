using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Index("PostsId", Name = "IX_Documents_PostsID")]
    [Index("ProfileId", Name = "IX_Documents_ProfileID")]
    public partial class Document
    {
        [Key]
        [Column("DocumentID")]
        public int DocumentId { get; set; }
        [Column("PostsID")]
        public int? PostsId { get; set; }
        [Column("ProfileID")]
        public int? ProfileId { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string FamilyType { get; set; } = null!;

        [ForeignKey("PostsId")]
        [InverseProperty("Documents")]
        public virtual Post? Posts { get; set; }
        [ForeignKey("ProfileId")]
        [InverseProperty("Documents")]
        public virtual Profile? Profile { get; set; }
    }
}
