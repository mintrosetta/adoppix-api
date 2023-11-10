using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("AuctionImages", Schema = "adoppix_admin")]
    public partial class AuctionImage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("auctionId")]
        [StringLength(450)]
        public string AuctionId { get; set; }
        [Column("imageTypeId")]
        public int ImageTypeId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(450)]
        public string Name { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("AuctionId")]
        [InverseProperty("AuctionImages")]
        public virtual Auction Auction { get; set; }
        [ForeignKey("ImageTypeId")]
        [InverseProperty("AuctionImages")]
        public virtual ImageType ImageType { get; set; }
    }
}
