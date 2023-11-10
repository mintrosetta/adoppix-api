namespace AdopPixAPI.DTOs
{
    public class AuctionDto : UserDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StopTime { get; set; }
        
        public decimal? CurrentBid { get; set; }
        public string UsernameCurrentBid { get; set; }

        public decimal MinimumBid { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public int Hour { get; set; }

        public List<AuctionBidHistory> BidHistories { get; set; }
        public List<string> Images { get; set; }
        public List<string> Tags { get; set; }
    }
}
