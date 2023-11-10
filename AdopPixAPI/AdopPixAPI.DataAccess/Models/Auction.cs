using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("Auctions", Schema = "adoppix_admin")]
    public partial class Auction
    {
        public Auction()
        {
            AuctionBids = new HashSet<AuctionBid>();
            AuctionImages = new HashSet<AuctionImage>();
            AuctionTags = new HashSet<AuctionTag>();
            AuctionWons = new HashSet<AuctionWon>();
        }

        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [Column("title")]
        [StringLength(50)]
        public string Title { get; set; }
        [Column("hour")]
        public int Hour { get; set; }
        [Required]
        [Column("description")]
        [StringLength(100)]
        public string Description { get; set; }
        [Column("start", TypeName = "datetime")]
        public DateTime? Start { get; set; }
        [Column("stop", TypeName = "datetime")]
        public DateTime? Stop { get; set; }
        [Column("openPrice", TypeName = "decimal(18, 2)")]
        public decimal OpenPrice { get; set; }
        [Column("closePrice", TypeName = "decimal(18, 2)")]
        public decimal? ClosePrice { get; set; }
        [Column("minimumBid", TypeName = "decimal(18, 2)")]
        public decimal MinimumBid { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime? Updated { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("jobId", TypeName = "text")]
        public string JobId { get; set; }
        [Column("isWon")]
        public bool? IsWon { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Auctions")]
        public virtual User User { get; set; }
        [InverseProperty("Auction")]
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }
        [InverseProperty("Auction")]
        public virtual ICollection<AuctionImage> AuctionImages { get; set; }
        [InverseProperty("Auction")]
        public virtual ICollection<AuctionTag> AuctionTags { get; set; }
        [InverseProperty("Auction")]
        public virtual ICollection<AuctionWon> AuctionWons { get; set; }
    }
}
