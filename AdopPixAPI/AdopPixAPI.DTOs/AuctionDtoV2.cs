namespace AdopPixAPI.DTOs
{
    public class AuctionDtoV2
    {
        public string AuctionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StopTime { get; set; }
        public string OwnerUsername { get; set; }
        public string OwnerProfileImage { get; set; }


        public decimal? CurrentBid { get; set; }
        public string UsernameCurrentBid { get; set; }

        public decimal MinimumBid { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public int Hour { get; set; }

        public List<string> Images { get; set; }
    }
}