using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class AuctionWonRepository : Repository<AuctionWon>, IAuctionWonRepository
    {
        public AuctionWonRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
