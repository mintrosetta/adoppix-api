using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("PostComments", Schema = "adoppix_admin")]
    public partial class PostComment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [Column("postId")]
        [StringLength(450)]
        public string PostId { get; set; }
        [Required]
        [Column("description")]
        [StringLength(100)]
        public string Description { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime? Updated { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("PostId")]
        [InverseProperty("PostComments")]
        public virtual Post Post { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("PostComments")]
        public virtual User User { get; set; }
    }
}
