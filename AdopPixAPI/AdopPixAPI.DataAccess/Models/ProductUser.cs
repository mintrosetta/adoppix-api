using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    public partial class ProductUser
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Required]
        [Column("productId")]
        [StringLength(450)]
        public string ProductId { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Column("price", TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Column("amount")]
        public int? Amount { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductUsers")]
        public virtual Product Product { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("ProductUsers")]
        public virtual User User { get; set; }
    }
}
