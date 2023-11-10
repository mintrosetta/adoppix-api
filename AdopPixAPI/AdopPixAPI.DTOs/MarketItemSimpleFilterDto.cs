using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace AdopPixAPI.DTOs
{
    public class MarketItemSimpleFilterDto
    {
        public string? Title { get; set; }
        public string? Tag { get; set; }
        public int? MarketItemType { get; set; }
        public string? UserId    { get; set; }
        public bool? Aged { get; set; }
        public bool? Popularity { get; set; }
        public decimal? StartPrice { get; set; }
        public decimal? EndPrice { get; set; }
    }
}
