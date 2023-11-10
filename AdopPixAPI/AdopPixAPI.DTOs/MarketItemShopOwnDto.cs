
namespace AdopPixAPI.DTOs
{
    public class MarketItemShopOwnDto
    {
        public string MarketItemId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal ProfitAmount { get; set; }
        public string MarketImage { get; set; }
        public int QuantitySold { get; set; }
        public int AmountLeft { get; set; }
        public int? MaxAmount { get; set; }
        public DateTime Created { get; set; }
    }
}
