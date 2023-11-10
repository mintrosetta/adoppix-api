using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("PostTags", Schema = "adoppix_admin")]
    public partial class PostTag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("postId")]
        [StringLength(450)]
        public string PostId { get; set; }
        [Required]
        [Column("tagId")]
        [StringLength(450)]
        public string TagId { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("PostId")]
        [InverseProperty("PostTags")]
        public virtual Post Post { get; set; }
        [ForeignKey("TagId")]
        [InverseProperty("PostTags")]
        public virtual Tag Tag { get; set; }
    }
}
