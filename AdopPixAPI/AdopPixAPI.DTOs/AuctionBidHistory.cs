namespace AdopPixAPI.DTOs
{
    public class AuctionBidHistory
    {
        public string ProfileImage { get; set; }
        public string Username { get; set; }
        public decimal AmountBid { get; set; }
        public DateTime Created { get; set; }
    }
}
