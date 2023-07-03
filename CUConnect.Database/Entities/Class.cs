using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUConnect.Database.Entities
{
    public partial class Class
    {
        public Class()
        {
            Enrollments = new HashSet<Enrollment>();
            Profiles = new HashSet<Profile>();
        }

        [Key]
        [Column("ClassID")]
        public int ClassId { get; set; }
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
        [StringLength(8)]
        public string Name { get; set; } = null!;
        public int Semester { get; set; }
        [StringLength(5)]
        public string Code { get; set; } = null!;

        [ForeignKey("DepartmentId")]
        [InverseProperty("Classes")]
        public virtual Department Department { get; set; } = null!;
        [InverseProperty("Class")]
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        [InverseProperty("Class")]
        public virtual ICollection<Profile> Profiles { get; set; }
    }
}
