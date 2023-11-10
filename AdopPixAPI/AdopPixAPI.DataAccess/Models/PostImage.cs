using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("PostImages", Schema = "adoppix_admin")]
    public partial class PostImage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("postId")]
        [StringLength(450)]
        public string PostId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(450)]
        public string Name { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("PostId")]
        [InverseProperty("PostImages")]
        public virtual Post Post { get; set; }
    }
}
