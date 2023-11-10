using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DTOs;

namespace AdopPixAPI.Services.IServices
{
    public interface IAuctionService
    {
        Task<string> CreateAsync(AuctionCreateDto dto, string userId);
        Task<AuctionDto> FindByIdAsync(string auctionId);
        Task<List<Auction>> FindByUserIdAsync(string userId, int take, int page, bool onlyActive);
        Task<List<string>> FindImageByAuctionId(string auctionId);
        Task<List<AuctionBid>> GetBidHistories(string auctionId);
        Task CreateBidAsync(string auctionId, decimal amount, string userId);
        Task<AuctionBid> GetMaxBidByAuctionIdAsync(string auctionId);
        Task Won(string userId, string auctionId, decimal amount);
        Task CreateEndAuctionTask(string auctionId, int time);
        Task<List<UserLoserAuctionDto>> GetLoser(string auctionId, string userIdWon);
        Task InitialTime(string auctionId, DateTime start, DateTime stop);
        Task<List<Auction>> GetAllAsync(int take, int page, bool onlyActive);
        void Test();
        Task<List<string>> FindTagById(string auctionId);
    }
}
