using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUConnect.Database.Entities
{
    public partial class Enrollment
    {
        [Key]
        [Column("EnrollmentID")]
        public int EnrollmentId { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; } = null!;
        [Column("ClassID")]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        [InverseProperty("Enrollments")]
        public virtual Class Class { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("Enrollments")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
