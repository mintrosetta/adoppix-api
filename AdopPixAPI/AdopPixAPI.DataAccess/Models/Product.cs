using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
            ProductTags = new HashSet<ProductTag>();
            ProductUsers = new HashSet<ProductUser>();
        }

        [Key]
        [Column("productId")]
        public string ProductId { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Column("productTypeId")]
        public int ProductTypeId { get; set; }
        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        [Column("description")]
        [StringLength(450)]
        public string Description { get; set; }
        [Column("price", TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Column("amount")]
        public int? Amount { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime Updated { get; set; }

        [ForeignKey("ProductTypeId")]
        [InverseProperty("Products")]
        public virtual ProductType ProductType { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Products")]
        public virtual User User { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductTag> ProductTags { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductUser> ProductUsers { get; set; }
    }
}
