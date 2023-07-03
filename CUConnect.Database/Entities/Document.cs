using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUConnect.Database.Entities
{
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
