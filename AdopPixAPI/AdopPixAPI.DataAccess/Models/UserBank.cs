using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("UserBanks", Schema = "adoppix_admin")]
    public partial class UserBank
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [Column("number")]
        [StringLength(50)]
        public string Number { get; set; }
        [Required]
        [Column("fullname")]
        [StringLength(50)]
        public string Fullname { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserBanks")]
        public virtual User User { get; set; }
    }
}
