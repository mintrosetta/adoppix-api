using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AdopPixAPI.DTOs
{
    public class AuctionCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Hour { get; set; }
        [Required]
        public decimal OpenPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        [Required]
        public decimal MinimumBit { get; set; }

        public List<string> Tags { get; set; }

        [Required]
        public List<IFormFile> Images { get; set; }
    }
}
