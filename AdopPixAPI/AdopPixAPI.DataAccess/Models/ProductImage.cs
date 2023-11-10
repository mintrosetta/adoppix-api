using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    public partial class ProductImage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("productId")]
        [StringLength(450)]
        public string ProductId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(450)]
        public string Name { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductImages")]
        public virtual Product Product { get; set; }
    }
}
