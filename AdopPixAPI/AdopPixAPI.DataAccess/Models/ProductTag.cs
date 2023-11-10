using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    public partial class ProductTag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("productId")]
        [StringLength(450)]
        public string ProductId { get; set; }
        [Required]
        [Column("tagId")]
        [StringLength(450)]
        public string TagId { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductTags")]
        public virtual Product Product { get; set; }
        [ForeignKey("TagId")]
        [InverseProperty("ProductTags")]
        public virtual Tag Tag { get; set; }
    }
}
