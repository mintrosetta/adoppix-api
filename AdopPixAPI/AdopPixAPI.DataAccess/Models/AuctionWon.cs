using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("AuctionWons", Schema = "adoppix_admin")]
    public partial class AuctionWon
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [Column("auctionId")]
        [StringLength(450)]
        public string AuctionId { get; set; }
        [Column("amount", TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("AuctionId")]
        [InverseProperty("AuctionWons")]
        public virtual Auction Auction { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("AuctionWons")]
        public virtual User User { get; set; }
    }
}
