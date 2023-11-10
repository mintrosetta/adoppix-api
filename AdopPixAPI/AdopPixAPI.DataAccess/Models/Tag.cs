using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("Tags", Schema = "adoppix_admin")]
    public partial class Tag
    {
        public Tag()
        {
            AuctionTags = new HashSet<AuctionTag>();
            PostTags = new HashSet<PostTag>();
            ProductTags = new HashSet<ProductTag>();
        }

        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Required]
        [Column("title")]
        [StringLength(20)]
        public string Title { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [InverseProperty("Tag")]
        public virtual ICollection<AuctionTag> AuctionTags { get; set; }
        [InverseProperty("Tag")]
        public virtual ICollection<PostTag> PostTags { get; set; }
        [InverseProperty("Tag")]
        public virtual ICollection<ProductTag> ProductTags { get; set; }
    }
}
