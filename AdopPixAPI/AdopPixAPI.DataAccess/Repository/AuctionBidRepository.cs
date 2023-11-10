using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class AuctionBidRepository : Repository<AuctionBid>, IAuctionBidRepository
    {
        public AuctionBidRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
