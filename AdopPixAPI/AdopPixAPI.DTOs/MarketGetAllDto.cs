using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdopPixAPI.DTOs
{
    public class MarketGetAllDto
    {
        // market item DB
        public string MarketID { get; set; }
        public int MarketTypeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Created { get; set; }

        // market item images DB
        public List<MarketItemImagesDto> MarketItemImages { get; set; }

        // market item tag DB
        public List<string>? MarketItemTags { get; set; }

        // market item sell amount
        public int ItemSellAmountID { get; set; }
        public int ItemSellTypeID { get; set; }
        public int? MaxAmount { get; set; }
        public int Amount { get; set; }

        // user who created market item
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
    }
}
