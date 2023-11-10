using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("AuctionTags", Schema = "adoppix_admin")]
    public partial class AuctionTag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("auctionId")]
        [StringLength(450)]
        public string AuctionId { get; set; }
        [Required]
        [Column("tagId")]
        [StringLength(450)]
        public string TagId { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("AuctionId")]
        [InverseProperty("AuctionTags")]
        public virtual Auction Auction { get; set; }
        [ForeignKey("TagId")]
        [InverseProperty("AuctionTags")]
        public virtual Tag Tag { get; set; }
    }
}
