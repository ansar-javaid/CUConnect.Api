using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Table("Profile")]
    [Index("ClassId", Name = "IX_Profile_ClassID")]
    [Index("DepartmentId", Name = "IX_Profile_DepartmentID")]
    [Index("UserId", Name = "IX_Profile_UserID")]
    public partial class Profile
    {
        public Profile()
        {
            Documents = new HashSet<Document>();
            Posts = new HashSet<Post>();
            Subscriptions = new HashSet<Subscription>();
        }

        [Key]
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("UserID")]
        public string UserId { get; set; } = null!;
        [Column("DepartmentID")]
        public int? DepartmentId { get; set; }
        [Column("ClassID")]
        public int? ClassId { get; set; }
        [StringLength(40)]
        public string Title { get; set; } = null!;
        [StringLength(300)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        [ForeignKey("ClassId")]
        [InverseProperty("Profiles")]
        public virtual Class? Class { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("Profiles")]
        public virtual Department? Department { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Profiles")]
        public virtual AspNetUser User { get; set; } = null!;
        [InverseProperty("Profile")]
        public virtual ICollection<Document> Documents { get; set; }
        [InverseProperty("Profile")]
        public virtual ICollection<Post> Posts { get; set; }
        [InverseProperty("Profile")]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
