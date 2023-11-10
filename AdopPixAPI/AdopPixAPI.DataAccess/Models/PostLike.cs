using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("PostLikes", Schema = "adoppix_admin")]
    public partial class PostLike
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Column("postId")]
        [StringLength(450)]
        public string PostId { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("PostId")]
        [InverseProperty("PostLikes")]
        public virtual Post Post { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("PostLikes")]
        public virtual User User { get; set; }
    }
}
