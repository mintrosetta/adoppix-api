using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdopPixAPI.DTOs
{
    public class MarketCreateDto
    {
        [Required]
        public List<IFormFile> ImgSet { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public int MarketTypeId { get; set; }
        [Required]
        public int SellType { get; set; }
        [Required]
        public int Amount { get; set; }
        public List<string> Tag { get; set; }
        [Required]
        public int Price { get; set; }
    }
}
