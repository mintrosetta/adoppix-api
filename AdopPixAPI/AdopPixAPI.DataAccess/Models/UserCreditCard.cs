using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("UserCreditCards", Schema = "adoppix_admin")]
    public partial class UserCreditCard
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
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column("number")]
        [StringLength(100)]
        public string Number { get; set; }
        [Required]
        [Column("expire")]
        [StringLength(10)]
        public string Expire { get; set; }
        [Required]
        [Column("type")]
        [StringLength(20)]
        public string Type { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserCreditCards")]
        public virtual User User { get; set; }
    }
}
