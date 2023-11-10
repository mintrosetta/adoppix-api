using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("Posts", Schema = "adoppix_admin")]
    public partial class Post
    {
        public Post()
        {
            PostComments = new HashSet<PostComment>();
            PostImages = new HashSet<PostImage>();
            PostLikes = new HashSet<PostLike>();
            PostTags = new HashSet<PostTag>();
        }

        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [Column("description")]
        [StringLength(450)]
        public string Description { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime? Updated { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Posts")]
        public virtual User User { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<PostComment> PostComments { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<PostImage> PostImages { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<PostLike> PostLikes { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}
