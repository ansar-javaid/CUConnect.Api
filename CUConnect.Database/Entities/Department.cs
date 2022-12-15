using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Table("Department")]
    public partial class Department
    {
        public Department()
        {
            Classes = new HashSet<Class>();
            Profiles = new HashSet<Profile>();
        }

        [Key]
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [InverseProperty("Department")]
        public virtual ICollection<Class> Classes { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<Profile> Profiles { get; set; }
    }
}
