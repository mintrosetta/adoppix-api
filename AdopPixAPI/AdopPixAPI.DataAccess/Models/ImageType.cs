using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("ImageTypes", Schema = "adoppix_admin")]
    public partial class ImageType
    {
        public ImageType()
        {
            AuctionImages = new HashSet<AuctionImage>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }

        [InverseProperty("ImageType")]
        public virtual ICollection<AuctionImage> AuctionImages { get; set; }
    }
}
