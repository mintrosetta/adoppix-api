using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("UserProfiles", Schema = "adoppix_admin")]
    public partial class UserProfile
    {
        [Key]
        [Column("userId")]
        public string UserId { get; set; }
        [Column("description")]
        [StringLength(200)]
        public string Description { get; set; }
        [Column("profileImage")]
        [StringLength(450)]
        public string ProfileImage { get; set; }
        [Column("coverImage")]
        [StringLength(450)]
        public string CoverImage { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime? Updated { get; set; }
        [Required]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; }
        [Column("money", TypeName = "decimal(18, 2)")]
        public decimal Money { get; set; }
        [Column("isDark")]
        public bool IsDark { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserProfile")]
        public virtual User User { get; set; }
    }
}
