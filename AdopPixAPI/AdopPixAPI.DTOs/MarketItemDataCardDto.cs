using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdopPixAPI.DTOs
{
    public class MarketItemDataCardDto
    {
        public string MarketItemId { get; set; }
        public string UserId { get; set; }
        public string UserName  { get; set; }

        public string UserImage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string MarketImage { get; set; }
        public DateTime Created { get; set; }
    }
}
