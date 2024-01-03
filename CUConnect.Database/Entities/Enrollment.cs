using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{
    [Index("ClassId", Name = "IX_Enrollments_ClassID")]
    [Index("UserId", Name = "IX_Enrollments_UserID")]
    public partial class Enrollment
    {
        [Key]
        [Column("EnrollmentID")]
        public int EnrollmentId { get; set; }
        [Column("UserID")]
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
